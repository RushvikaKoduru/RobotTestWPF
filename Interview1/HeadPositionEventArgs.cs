using System;

namespace Interview1
{
    /// <summary>
    /// A class representing the event arguments raised by a <see cref="Robot.Moving"/> event.
    /// </summary>
    public class RobotPositionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the current position of the robot.
        /// </summary>
        public Position CurrentPosition { get; private set; }

        /// <summary>
        /// Gets the time until the robot will be on-target.
        /// </summary>
        public TimeSpan TimeToShot { get; private set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="RobotPositionEventArgs"/> class.
        /// </summary>
        /// <param name="currentPosition">The current position of the robotic device.</param>
        /// <param name="timeToShot">The time until the shot will be on target.</param>
        public RobotPositionEventArgs(Position currentPosition, TimeSpan timeToShot)
        {
            CurrentPosition = currentPosition;
            TimeToShot = timeToShot;
        }
    }
}
