namespace Interview1
{
    using System;

    /// <summary>
    /// A class representing the event arguments raised by a <see cref="Robot.StatusChanged"/> event.
    /// </summary>
    public class StatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a value indicating the current robot state.
        /// </summary>
        public RobotStatus Status { get; private set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="StatusChangedEventArgs"/> class.
        /// </summary>
        /// <param name="status">The new status.</param>
        public StatusChangedEventArgs(RobotStatus status)
        {
            Status = status;
        }
    }
}
