namespace Interview1
{
    /// <summary>
    /// The Shot class stores the position for a robot.
    /// </summary>
    public class Shot
    {
        /// <summary>
        /// Gets the head id for the shot.
        /// </summary>
        public string RobotId { get; private set; }

        /// <summary>
        /// Gets the position for the shot.
        /// </summary>
        public Position Position { get; private set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="Shot"/> class.
        /// </summary>
        /// <param name="robotId">The id of the robot that this shot is for.</param>
        /// <param name="position">The shot position.</param>
        public Shot(string robotId, Position position)
        {
            RobotId = robotId;
            Position = position;
        }
    }
}
