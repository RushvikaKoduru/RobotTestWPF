using System.Collections.Generic;

namespace Interview1
{
    /// <summary>
    /// Studio model. Provides Robots and Targets.
    /// </summary>
    public interface IStudio
    {
        IEnumerable<IRobot> Robots { get; }

        IEnumerable<Target> Targets { get; }
    }
}
