using Interview1;

namespace TestApp.ViewModels.Interfaces
{
    public interface IRobotViewModelFactory
    {
        IRobotViewModel Create(IRobot target);
    }
}