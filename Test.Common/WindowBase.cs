using OpenQA.Selenium;
using System;
using System.Drawing;

namespace Test.Common
{
    public abstract class WindowBase : WindowsElementBase
    {
        public enum WindowState
        {
            Maximized,
            Minimized,
            Normal
        }

        protected WindowBase(string automationId) : base(TestInstance.Instance.WindowsDriver.TryGetElement(automationId, RetryInterval.None))
        {
        }

        public override Size GetSize => WindowsDriver.Manage().Window.Size;

        public override Point GetLocation => WindowsDriver.Manage().Window.Position;

        public void Maximise()
        {
            WindowsDriver.Manage().Window.Maximize();
        }

        public WindowState GetWindowState()
        {
            var state = GetAttributeFromPageSource("Window", "WindowVisualState", false);

            return (WindowState)Enum.Parse(typeof(WindowState), state, true);
        }

        public abstract void CloseWindow();

        /// <summary>
        /// Attempts to close the application by using the X in the title bar.
        /// Failing that, harder close methods are attempted ending in a harsh kill.
        /// </summary>
        protected void ClickXAndConfirmClose(Action closeAction)
        {
            try
            {
                closeAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error exiting cleanly: " + e);
            }
        }

        public void SetSize(int width, int height)
        {
            var size = new Size(width, height);
            WindowsDriver.Manage().Window.Size = size;
        }

        public void CloseByAltF4()
        {
            WindowsDriver.Keyboard.SendKeys(Keys.Alt + Keys.F4);
            WindowsDriver.Keyboard.ReleaseKey(Keys.Alt + Keys.F4);
        }
    }
}