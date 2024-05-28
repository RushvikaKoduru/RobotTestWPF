using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Interview1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TestApp.ViewModels.Interfaces;

namespace TestApp.ViewModels
{
    /// <summary>
    /// Model for the app's main studio view, consisting of targets, robots and the move and stop command that can be applied to all the robots
    /// </summary>
    public class StudioViewModel : ViewModelBase
    {
        private readonly RelayCommand _moveAllCommand;
        private readonly RelayCommand _stopAllCommand;
        private ITargetViewModel _selectedTarget;

        /// <summary>
        /// Initializes a new instance of the StudioViewModel class.
        /// </summary>
        public StudioViewModel(IStudio studio, IRobotViewModelFactory robotMoveModelFactory)
        {
            Robots = studio.Robots.Select(robot => robotMoveModelFactory.Create(robot)).ToList();
            Targets = studio.Targets.Select(target => new TargetViewModel(target)).Cast<ITargetViewModel>().ToList();
            _moveAllCommand = new RelayCommand(MoveAllCommandExecute, MoveAllCommandCanExecute);
            _stopAllCommand = new RelayCommand(StopAllCommandExecute, StopAllCommandCanExecute);
            MessengerInstance.Register<RobotStatusUpdate>(this, OnRobotStatusChanged);
        }

        /// <summary>
        /// Move all robots to target command
        /// </summary>
        public ICommand MoveAllCommand => _moveAllCommand;

        /// <summary>
        /// Stop all robots moving command
        /// </summary>
        public ICommand StopAllCommand => _stopAllCommand;

        /// <summary>
        /// The targets configured in the studio
        /// </summary>
        public IList<ITargetViewModel> Targets { get; protected set; }

        /// <summary>
        /// The robots configured in the studio
        /// </summary>
        public IList<IRobotViewModel> Robots { get; protected set; }

        /// <summary>
        /// Any messages that have been received from the robots
        /// </summary>
        public ObservableCollection<MessageViewModel> Messages { get; } = new ObservableCollection<MessageViewModel>();

        /// <summary>
        /// The selected target for the next move command
        /// </summary>
        public ITargetViewModel SelectedTarget
        {
            get => _selectedTarget;
            set
            {
                if (Set(() => SelectedTarget, ref _selectedTarget, value))
                {
                    MessengerInstance.Send(_selectedTarget);
                }
            }
        }

        /// <summary>
        /// MVVM light shut down clean up
        /// </summary>
        public override void Cleanup()
        {
            foreach (var robot in Robots)
            {
                robot.Cleanup();
            }

            base.Cleanup();
        }

        private void OnRobotStatusChanged(RobotStatusUpdate robotStatusUpdate)
        {
            if (!string.IsNullOrWhiteSpace(robotStatusUpdate.Message))
            {
                Messages.Insert(0, new MessageViewModel
                {
                    Time = DateTime.Now,
                    Name = robotStatusUpdate.Name,
                    Message = robotStatusUpdate.Message
                });
            }

            _moveAllCommand.RaiseCanExecuteChanged();
            _stopAllCommand.RaiseCanExecuteChanged();
        }

        private bool MoveAllCommandCanExecute()
        {
            return Robots.Any(r => r.MoveCommand.CanExecute(null));
        }

        private void MoveAllCommandExecute()
        {
            Robots.RunCommandForAll(robot => robot.MoveCommand);
        }

        private bool StopAllCommandCanExecute()
        {
            return Robots.Any(r => r.StopCommand.CanExecute(null));
        }

        private void StopAllCommandExecute()
        {
            Robots.RunCommandForAll(robot => robot.StopCommand);
        }

    }
}