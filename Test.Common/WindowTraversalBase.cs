using OpenQA.Selenium.Appium.Windows;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Test.Common
{
    public abstract class WindowTraversalBase
    {
        [DllImport("User32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);

        protected WindowBase GetWindow(Func<WindowBase> getWindow, string windowId)
        {
            const int timeOut = 120000;
            string message = "Error switching current window  within the time specified {0}. Stack trace: ";
            var errorText = string.Empty;

            WindowBase window = null;

            var retryCount = 0;

            try
            {
                FunctionRunner.RunFuncUntilSuccess(() =>
                {
                    try
                    {
                        if (TestInstance.Instance.WindowsDriver.WindowHandles.Any())
                        {
                            var currentHandle = TestInstance.Instance.WindowsDriver.WindowHandles[0];
                            var newDriverInstance = TestInstance.Instance.WindowsDriver.SwitchTo().Window(currentHandle);

                            TestInstance.Instance.WindowsDriver = (WindowsDriver<WindowsElement>)newDriverInstance;

                            window = getWindow();
                            TestInstance.Instance.CurrentWindow = window;

                            var windowHandle = window.Element.GetAttribute("NativeWindowHandle");

                            SetForegroundWindow(new IntPtr(long.Parse(windowHandle)));
                            return true;
                        }

                        // throw exception so that WinApp driver will be re-attached in the catch block
                        throw new Exception("No window handles!");
                    }
                    catch (Exception e)
                    {
                        errorText = message + e.StackTrace;

                        // If here an error has occurred with WinApp driver, so attempt reattach to application window
                        // If failed more than 5 times then kill WinApp driver and relaunch
                        retryCount++;
                        if (retryCount > 5)
                        {
                            ProcessManagement.KillAndLaunchAppium();
                            retryCount = 0;
                        }

                        TestInstance.Instance.AttachWinAppDriverToApplication(windowId);

                        return false;
                    }
                    // ReSharper disable once ImplicitlyCapturedClosure
                }, () => errorText, timeOut);
            }
            catch (TimeoutException)
            {
                // If here then there is no chance for testing to continue. All attempts to re-attach WinApp driver have failed.
                // So kill the application and the WinApp driver instance so the next test can continue
                ProcessManagement.KillProcess(TestInstance.Instance.ProcessName);
                ProcessManagement.KillAndLaunchAppium();

                throw;
            }

            return window;
        }
    }
}
