using TestApp.ViewModels.Interfaces;

namespace TestApp.ViewModels
{
    public class RobotMoveModelFactory : IRobotMoveModelFactory
    {
        public IRobotMoveModel Create(ITargetViewModel target)
        {
            return new RobotMoveModel(target);
        }
    }
}