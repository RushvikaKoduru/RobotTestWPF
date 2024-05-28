using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;


namespace Test.Common
{
    public class TestInstance
    {
        public const int TimeToWaitForApplicationExit = 10000;
        public WindowsDriver<WindowsElement> WindowsDriver { get; set; }
        public WindowBase CurrentWindow { get; set; }

        public static TestInstance Instance { get; } = new TestInstance();

        private TestInstance()
        {

        }

        public string ProcessName { get; private set; }

        /// <summary>
        /// StartUpWindow is used as a fallback when the primary attempt has failed to locate the window. 
        /// This typically happens when launching the installers as they have two processes with the same name and Appium fails to attach to the correct process.
        /// (This is normal for Wix installers. One process for the ui and one for elevated permissions)
        /// </summary>
        protected string[] StartUpWindow { get; }

        public void Initialise(string processName, string exePath, string commandLineParameters = "")
        {
            ProcessName = processName;

            var appCapabilities = new DesiredCapabilities();
            appCapabilities.SetCapability("platformName", "Windows");
            appCapabilities.SetCapability("platformVersion", "10");
            appCapabilities.SetCapability("deviceName", "WindowsPC");
            appCapabilities.SetCapability("app", exePath);
            if (!string.IsNullOrWhiteSpace(commandLineParameters))
            {
                appCapabilities.SetCapability("appArguments", commandLineParameters);
            }
            appCapabilities.SetCapability("appWorkingDir", Path.GetDirectoryName(exePath));

            InitialiseDriver(appCapabilities);
        }

        private void InitialiseDriver(DesiredCapabilities appCapabilities)
        {
            var timeOut = 120000;
            var message = "Error initialising Windows driver within the time specified {0}. Stack trace: ";
            var errorText = string.Empty;

            ProcessManagement.LaunchAppiumIfNotRunning();

            FunctionRunner.RunFuncUntilSuccess(() =>
            {
                try
                {
                    // Try Primary method to launch and attach to process
                    WindowsDriver = CreateWindowsDriver(appCapabilities, ApplicationUri);

                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error attaching to process...trying alternate method");

                    // Now try fallback routine and try to attach to the startup window
                    if (!AttachWinAppDriverToApplication(StartUpWindow))
                    {
                        ProcessManagement.KillProcess(ProcessName);

                        errorText = message + e.StackTrace;

                        return false;
                    }

                    return true;
                }
                // ReSharper disable once ImplicitlyCapturedClosure
            }, () => errorText, timeOut);
        }

        private Uri ApplicationUri => new Uri($"http://127.0.0.1:4723"); //"http://127.0.0.1:4723/wd/hub";

        public bool AttachWinAppDriverToApplication(params string[] windows)
        {
            try
            {
                foreach (var window in windows)
                {
                    try
                    {
                        // Attach to the desktop 
                        var appAlternateCapabilities = new DesiredCapabilities();
                        appAlternateCapabilities.SetCapability("app", "Root");
                        WindowsDriver = CreateWindowsDriver(appAlternateCapabilities, ApplicationUri);

                        // Find the window
                        var applicationWindow = WindowsDriver.TryGetElement(window, RetryInterval.Extended);
                        var applicationWindowHandle = applicationWindow.GetAttribute("NativeWindowHandle");
                        applicationWindowHandle = (int.Parse(applicationWindowHandle)).ToString("x"); // Convert to Hex

                        // Create session by attaching to Window 
                        appAlternateCapabilities = new DesiredCapabilities();
                        appAlternateCapabilities.SetCapability("appTopLevelWindow", applicationWindowHandle);
                        WindowsDriver = CreateWindowsDriver(appAlternateCapabilities, ApplicationUri);

                        // If successful then break out of loop
                        return true;
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                throw new Exception("Window could not be found");
            }
            catch (Exception)
            {
                Console.WriteLine("Error attaching to process...retrying");

                // Kill process and try again
                ProcessManagement.KillAndLaunchAppium();

                return false;
            }
        }

        private WindowsDriver<WindowsElement> CreateWindowsDriver(DesiredCapabilities appCapabilities, Uri applicationUri)
        {
            var windowsDriver = new WindowsDriver<WindowsElement>(applicationUri, appCapabilities, Timeout.InfiniteTimeSpan);
            windowsDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            return windowsDriver;
        }

        public void WaitForApplicationExit()
        {
            try
            {
                ApplicationIsRunning(false);
            }
            catch (Exception)
            {
                // Application still running so dispose of WinAppDriver
                WindowsDriver.Dispose();

                // Kill the application
                ProcessManagement.KillProcess(ProcessName);
            }
        }

        public void ApplicationIsRunning(bool state)
        {
            var message = state ? "Application running state not detected" : "Application exit state not detected";

            FunctionRunner.RunFuncUntilSuccess(() =>
            {
                try
                {
                    return WindowsDriver.WindowHandles != null && WindowsDriver.WindowHandles.Any() && WindowsDriver.Title != null ? state : !state;
                }
                catch (Exception)
                {
                    return !state;
                }
            }, () => message + " within the allocated time {0} milliseconds", TimeToWaitForApplicationExit);
        }

        public void CloseApp()
        {
            WindowsDriver.CloseApp();
        }

        public Bitmap GetBitmapFromScreenshot(Screenshot screenshot)
        {
            return Image.FromStream(new MemoryStream(screenshot.AsByteArray)) as Bitmap;
        }

        public Bitmap GetBitmapFromWindowsElement(WindowsElement element, string filePath)
        {
            TakeScreenshot(filePath);
            try
            {
                return GetBitmapFromScreenshot(element.GetScreenshot());
            }
            catch (InvalidOperationException e)
            {
                var image = GetBitmapFromScreenshot(WindowsDriver.GetScreenshot());
                var cloneRect = new Rectangle(element.Location.X, element.Location.Y, element.Size.Width, element.Size.Height);
                var format = image.PixelFormat;
                return image.Clone(cloneRect, format);
            }
        }

        public void TakeScreenshot(string filepath)
        {
            var screenshot = GetBitmapFromScreenshot(WindowsDriver.GetScreenshot());
            screenshot.Save(filepath, ImageFormat.Png);
        }
    }
}
