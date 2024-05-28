using Interview1;
using System;
using TestApp.ViewModels.Interfaces;

namespace TestApp.ViewModels
{
    public class RobotViewModelFactory : IRobotViewModelFactory
    {
        private readonly IRobotMoveModelFactory robotMoveModelFactory;

        public RobotViewModelFactory(IRobotMoveModelFactory robotMoveModelFactory)
        {
            this.robotMoveModelFactory = robotMoveModelFactory ?? throw new ArgumentNullException(nameof(robotMoveModelFactory));
        }

        public IRobotViewModel Create(IRobot robot)
        {
            return new RobotViewModel(robot, robotMoveModelFactory);
        }
    }
}