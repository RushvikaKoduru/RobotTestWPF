using Interview1;

namespace TestApp.ViewModels.Interfaces
{
    public interface ITargetViewModel
    {
        string Name { get; }

        void Cleanup();

        Position GetPositionForRobot(string robotId);
    }
}