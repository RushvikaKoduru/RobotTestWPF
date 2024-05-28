using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Interview1;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TestApp.ViewModels.Interfaces;

namespace TestApp.ViewModels
{
    /// <summary>
    /// Models a robot in the studio view
    /// </summary>
    public class RobotViewModel : ViewModelBase, IRobotViewModel
    {
        private const int MinTicksToShot = 1;
        private static readonly object _robotMoveLock = new object();

        private readonly IRobotMoveModelFactory _robotMoveModelFactory;
        private readonly RelayCommand _moveCommand;
        private readonly RelayCommand _stopCommand;

        private TimeSpan _timeToShot = TimeSpan.Zero;
        private long _maxTicksToShot = 1;
        private IRobot _robot;
        private RobotStatus _status;
        private IRobotMoveModel _robotMoveModel;
        private ITargetViewModel _selectedTarget;

        /// <summary>
        /// Initializes a new instance of the RobotViewModel class.
        /// </summary>
        public RobotViewModel(IRobot robot, IRobotMoveModelFactory robotMoveModelFactory)
        {
            _robot = robot ?? throw new ArgumentNullException(nameof(robot));
            _robotMoveModelFactory = robotMoveModelFactory ?? throw new ArgumentNullException(nameof(robotMoveModelFactory));
            _robotMoveModel = _robotMoveModelFactory.Create(null);
            robot.OnStatusChanged += Robot_OnStatusChanged;
            robot.OnPositionChanged += Robot_OnPositionChanged;
            _moveCommand = new RelayCommand(MoveCommandExecute, MoveCommandCanExecute);
            _stopCommand = new RelayCommand(StopCommandExecute, StopCommandCanExecute);
            MessengerInstance.Register<ITargetViewModel>(this, OnSelectedTargetChanged);
        }

        /// <summary>
        /// The name (id) of the robot
        /// </summary>
        public string Name => _robot?.Name;

        /// <summary>
        /// The robot's current status
        /// </summary>
        public RobotStatus Status
        {
            get => _status;
            private set => Set(() => Status, ref _status, value);
        }

        /// <summary>
        /// The time remaining till the robot is on target
        /// If it's greater than the current max times taken then MaxTicksToShot is reset
        /// </summary>
        public TimeSpan TimeToShot
        {
            get => _timeToShot;
            private set
            {
                MaxTicksToShot = Math.Max(MaxTicksToShot, value.Ticks);
                Set(() => TimeToShot, ref _timeToShot, value);
            }
        }

        /// <summary>
        /// The maximum number of ticks the robot has reported for time to shot
        /// </summary>
        public long MaxTicksToShot
        {
            get => _maxTicksToShot;
            private set => Set(() => MaxTicksToShot, ref _maxTicksToShot, value);
        }

        /// <summary>
        /// Move robot to currently selected target command
        /// </summary>
        public ICommand MoveCommand => _moveCommand;

        /// <summary>
        /// Stop robot moving command
        /// </summary>
        public ICommand StopCommand => _stopCommand;

        /// <summary>
        /// MVVM light shut down clean up
        /// </summary>
        public override void Cleanup()
        {
            _selectedTarget = null;
            var robot = _robot;
            if (robot != null)
            {
                _robot = null;
                robot.OnStatusChanged -= Robot_OnStatusChanged;
            }

            base.Cleanup();
        }

        private void OnSelectedTargetChanged(ITargetViewModel targetViewModel)
        {
            _selectedTarget = targetViewModel;
            UpdateCommands();
        }

        private void UpdateCommands()
        {
            _moveCommand.RaiseCanExecuteChanged();
            _stopCommand.RaiseCanExecuteChanged();
            MessengerInstance.Send(new RobotStatusUpdate { Name = Name });
        }

        private bool MoveCommandCanExecute()
        {
            bool canExecute = _selectedTarget != null && !_robotMoveModel.IsInProgressTo(_selectedTarget);
            return canExecute;
        }

        private async void MoveCommandExecute()
        {
            var robotName = Name;
            if (!string.IsNullOrWhiteSpace(robotName))
            {
                try
                {
                    var targetViewModel = _selectedTarget;
                    using (var robotMoveModel = _robotMoveModelFactory.Create(targetViewModel))
                    {
                        IRobotMoveModel previousRobotMoveModel = null;
                        lock (_robotMoveLock)
                        {
                            if (_robotMoveModel.IsInProgressTo(targetViewModel))
                            {
                                Debug.WriteLine($"*{Name} was already in progress to {targetViewModel?.Name}");
                            }
                            else
                            {
                                previousRobotMoveModel = Interlocked.Exchange(ref _robotMoveModel, robotMoveModel);
                            }
                        }

                        if (previousRobotMoveModel != null)
                        {
                            await previousRobotMoveModel.Cancel();
                            TimeToShot = TimeSpan.Zero;
                            MaxTicksToShot = MinTicksToShot;
                            UpdateCommands();
                            await robotMoveModel.MoveRobotToTarget(_robot);
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    Debug.WriteLine($"*{Name} move cancelled");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"*{Name} move errored {ex.Message}");
                    MessengerInstance.Send(new RobotStatusUpdate { Name = Name, Message = (ex.InnerException ?? ex).Message });
                }

                UpdateCommands();
            }
        }

        private bool StopCommandCanExecute()
        {
            bool canExecute = _robotMoveModel.IsInProgress;
            return canExecute;
        }

        private async void StopCommandExecute()
        {
            await _robotMoveModel.Cancel();
            TimeToShot = TimeSpan.Zero;
        }

        private void Robot_OnStatusChanged(object sender, StatusChangedEventArgs e)
        {
            Status = e.Status;
        }

        private void Robot_OnPositionChanged(object sender, RobotPositionEventArgs e)
        {
            TimeToShot = e.TimeToShot;
        }
    }
}