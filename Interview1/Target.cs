using System.Linq;

namespace Interview1
{
    /// <summary>
    /// The target class represents a position within a studio or conference hall, this may be a weather map, a clock or a seat.
    /// </summary>
    /// <remarks>
    /// A target can have stored one or more shots, with each shot representing a single robot position (pan/tilt), used to point to a target.
    /// </remarks>
    public class Target
    {
        /// <summary>
        /// Gets the unique name for the target.
        /// </summary>
        /// <remarks>
        /// The name of the target is only guaranteed to be unique within the studio.
        /// </remarks>
        public string Name { get; private set; }

        /// <summary>
        /// Gets all of the shots stored for a target.
        /// </summary>
        public Shot[] Shots { get; private set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="Target"/> class.
        /// </summary>
        /// <param name="name">Unique name of the target</param>
        /// <param name="shots">Shots pointing at the target (1 per robot).</param>
        public Target(string name, Shot[] shots)
        {
            Name = name;
            Shots = shots;
        }

        /// <summary>
        /// Gets the shot stored for a specific robot.
        /// </summary>
        /// <param name="robotId">The robot id to search for the shot.</param>
        /// <returns>The robot shot information, if no shot is stored for the camera head then null is returned.</returns>
        public Shot GetShotForCamera(string robotId)
        {
            return (from s in Shots where s.RobotId.Equals(robotId) select s).SingleOrDefault();
        }

    }
}
