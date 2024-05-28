namespace TestApp.ViewModels.Interfaces
{
    public interface IRobotMoveModelFactory
    {
        IRobotMoveModel Create(ITargetViewModel target);
    }
}