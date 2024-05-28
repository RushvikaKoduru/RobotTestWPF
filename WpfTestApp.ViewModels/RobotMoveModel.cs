using Interview1;
using System;
using System.Threading;
using System.Threading.Tasks;
using TestApp.ViewModels.Interfaces;

namespace TestApp.ViewModels
{
    /// <summary>
    /// Models one robot being moved to the one target
    /// </summary>
    internal class RobotMoveModel : IRobotMoveModel
    {
        private static readonly object _cancellationLock = new object();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private ITargetViewModel _target;
        private Task _moveToPositionTask;

        /// <summary>
        /// Constructor that takes the target of the move
        /// </summary>
        /// <param name="target"></param>
        public RobotMoveModel(ITargetViewModel target)
        {
            _target = target;
        }

        /// <summary>
        /// Is in progress if we have a target but no move started yet, or the move has completed
        /// </summary>
        public bool IsInProgress => !string.IsNullOrEmpty(_target?.Name) && !(_moveToPositionTask?.IsCompleted ?? false);

        /// <summary>
        /// Sends the cancellation request and awaits the effect on any move that's in progress
        /// </summary>
        /// <returns></returns>
        public async Task Cancel()
        {
            try
            {
                lock (_cancellationLock)
                {
                    if (!_cancellationTokenSource.IsCancellationRequested)
                    {
                        _cancellationTokenSource.Cancel();
                    }
                }

                await (_moveToPositionTask ?? Task.FromResult(0));
            }
            catch (ObjectDisposedException)
            {
                // no need to cancel this one it's been disposed already
            }
            catch (Exception)
            {
                // we just want to wait on the move task cancellation; any exceptions will handled elsewhere
            }
        }

        /// <summary>
        /// Returns true if a target is selected and this robot move is in progress to that target
        /// </summary>
        /// <param name="selectedTarget"></param>
        /// <returns></returns>
        public bool IsInProgressTo(ITargetViewModel selectedTarget)
        {
            return !string.IsNullOrWhiteSpace(selectedTarget?.Name) && selectedTarget.Name.Equals(_target?.Name, StringComparison.InvariantCultureIgnoreCase) && IsInProgress;
        }

        /// <summary>
        /// Moves the given robot to the target for this movement
        /// </summary>
        /// <param name="robot"></param>
        /// <returns></returns>
        public Task MoveRobotToTarget(IRobot robot)
        {
            if (_moveToPositionTask != null)
            {
                throw new InvalidOperationException($"{nameof(MoveRobotToTarget)} can only be called once per instance.");
            }

            lock (_cancellationLock)
            {
                if (robot != null && !_cancellationTokenSource.IsCancellationRequested)
                {
                    var destination = _target?.GetPositionForRobot(robot.Name);
                    if (destination != null)
                    {
                        _cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(1));
                        _moveToPositionTask = robot.MoveToPosition(destination, _cancellationTokenSource.Token) ?? Task.FromResult(0);
                    }
                }
            }

            return _moveToPositionTask ?? Task.FromResult(0);
        }

        public void Dispose()
        {
            _target = null;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}