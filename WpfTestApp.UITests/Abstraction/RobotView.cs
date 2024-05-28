using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using Test.Common;
using Test.Common.Controls;

namespace WpfTestApp.UITests.Abstraction
{
    public class RobotView : ViewBase
    {
        public RobotView(Element element) : base(element)
        {
        }

        //public Button GoButton => GetButtonXPaths("//Button[@Name='Go'])[1]");
        public Button GoButton => GetButton("GoButton");
        public Button StopButton => GetButton("StopButton");
        //public ProgressBar ProgressBar => GetProgress("ProgressBar");


        public string Status => new TextBlock(this.TryGetElement("RobotStatus")).Text;
        public string Name => new TextBlock(this.TryGetElement("RobotName")).Text;




        private Button GetButton(string automationId) => new Button(this.TryGetElement(automationId));
        //private Button GetButtonXPaths(string automationId) => new Button(this.TryGetElement(automationId));

        //public ProgressBar GetProgress(string automationId) => new ProgressBar((RemoteWebElement)this.TryGetElement(automationId));
        
       
    }
}
