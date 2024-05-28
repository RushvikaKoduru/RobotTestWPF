using Interview1;
using System;
using System.Threading.Tasks;

namespace TestApp.ViewModels.Interfaces
{
    public interface IRobotMoveModel : IDisposable
    {
        bool IsInProgress { get; }
        bool IsInProgressTo(ITargetViewModel selectedTarget);
        Task MoveRobotToTarget(IRobot robot);
        Task Cancel();
    }
}