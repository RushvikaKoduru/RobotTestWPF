using System;
using System.Threading;
using System.Threading.Tasks;

namespace Interview1
{
    public interface IRobot
    {
        /// <summary>
        /// Gets the name of the robot, the name is unique.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Asynchronous method to tell the robot to move to a position.
        /// The task will complete when the move completes.
        /// Exception will be thrown if:
        ///         - this method is called when a previous move is still in progress
        ///         - the supplied position is outside the limits of the robot
        ///         - a robot error occurs during the move
        /// </summary>
        /// <param name="position">The new position to send the robot to.</param>
        /// <param name="cs">To cancel the move</param>
        Task MoveToPosition(Position position, CancellationToken cs);

        /// <summary>
        /// Event raised when the robot position changes.
        /// </summary>
        event EventHandler<RobotPositionEventArgs> OnPositionChanged;

        /// <summary>
        /// Event raised when the status of the robot changes.
        /// </summary>
        event EventHandler<StatusChangedEventArgs> OnStatusChanged;
    }
}
