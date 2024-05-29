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

        public Button GoButton => GetButton("GoButton");
        public Button StopButton => GetButton("StopButton");


        public string Status => new TextBlock(this.TryGetElement("RobotStatus")).Text;
        public string Name => new TextBlock(this.TryGetElement("RobotName")).Text;




        private Button GetButton(string automationId) => new Button(this.TryGetElement(automationId));
        
       
    }
}
