using System;
using OpenQA.Selenium.Appium.Windows;

namespace Test.Common.Controls
{
    public class Button : WindowsElementBase
    {
        public string Caption => Element.Text;

        public Button()
        {
        }

        public Button(Element element) : base(element)
        {
        }
    }
}
