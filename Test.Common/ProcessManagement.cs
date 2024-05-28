using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;

namespace Test.Common
{
    public static class ProcessManagement
    {
        public const string AppiumProcessName = "WinAppDriver";
        public const string AppiumClient = @"C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe";

        public static void LaunchAppiumIfNotRunning()
        {
            if (!ProcessIsRunning(AppiumProcessName))
            {
                Console.WriteLine("Appium not running...now launching!");
                LaunchAppium();
            }
            else
            {
                Console.WriteLine("Appium is running");
            }
        }
        public static bool ProcessIsRunning(string process)
        {
            var proc = Process.GetProcessesByName(process).FirstOrDefault();
            return proc != null && !proc.HasExited;
        }

        private static void LaunchAppium()
        {
            try
            {
                if (!IsAdministrator())
                {
                    throw new InvalidOperationException("Process must be running as administrator in order to launch Appium");
                }

                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = AppiumClient,
                        Arguments = "127.0.0.1 4723",
                        WindowStyle = ProcessWindowStyle.Minimized,
                    }
                };

                process.Start();
                if (process.HasExited)
                {
                    throw new InvalidOperationException($"Appium Process has exited {process.ExitCode:X}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to start Appium {e.Message}");
                throw;
            }
        }

        private static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void KillProcess(string processName)
        {
            Process proc;
            var skippedIds = new List<int>();

            do
            {
                proc = Process.GetProcessesByName(processName).FirstOrDefault(x => !skippedIds.Contains(x.Id));

                if (proc != null)
                {
                    try
                    {
                        if (!proc.HasExited) Delay.Medium();
                        Console.WriteLine("Process still alive. Attempting process kill...");
                        if (!proc.HasExited) proc.Kill();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Process suspended/locked. {e.Message}. Skipping ");
                        skippedIds.Add(proc.Id);
                    }
                }

            } while (proc != null);
        }

        public static void KillAndLaunchAppium()
        {
            KillProcess(AppiumProcessName);

            LaunchAppium();
        }

        public static void LaunchProcess(string filename, string workingDirectory)
        {
            try
            {
                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = filename,
                        WorkingDirectory = workingDirectory
                    }
                };

                process.Start();
                if (process.HasExited)
                {
                    throw new InvalidOperationException($"Process has exited {process.ExitCode:X}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to start Process {e.Message}");
                throw;
            }
        }
    }
}
