using Interview1;
using System;
using System.Windows.Input;

namespace TestApp.ViewModels.Interfaces
{
    public interface IRobotViewModel
    {
        ICommand MoveCommand { get; }

        string Name { get; }

        RobotStatus Status { get; }

        ICommand StopCommand { get; }

        TimeSpan TimeToShot { get; }

        void Cleanup();
    }
}