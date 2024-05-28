namespace Interview1
{
    public class Position
    {
        /// <summary>
        /// Gets the current pan position.
        /// </summary>
	    public double Pan { get; private set; }

        /// <summary>
        /// Gets the current tilt position.
        /// </summary>
        public double Tilt { get; private set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="Position"/> class.
        /// </summary>
        /// <param name="pan">The pan value.</param>
        /// <param name="tilt">The tilt value.</param>
	    public Position(double pan, double tilt)
        {
            Pan = pan;
            Tilt = tilt;
        }
    }
}
