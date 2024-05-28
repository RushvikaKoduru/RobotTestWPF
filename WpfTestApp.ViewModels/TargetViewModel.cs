using GalaSoft.MvvmLight;
using Interview1;
using System;
using TestApp.ViewModels.Interfaces;

namespace TestApp.ViewModels
{
    /// <summary>
    /// Models a target in the studio view
    /// </summary>
    public class TargetViewModel : ViewModelBase, ITargetViewModel
    {
        private readonly Target _target;

        /// <summary>
        /// Initializes a new instance of the TargetViewModel class.
        /// </summary>
        /// <param name="target"></param>
        public TargetViewModel(Target target)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        /// <summary>
        /// The name of the target
        /// </summary>
        public string Name => _target.Name;

        /// <summary>
        /// Gets position of this target for the robot with the given id
        /// </summary>
        /// <param name="robotId"></param>
        /// <returns>Can return null when no shot is available</returns>
        public Position GetPositionForRobot(string robotId)
        {
            return _target.GetShotForCamera(robotId)?.Position;
        }
    }
}