using System.Collections.Generic;
using System;
using System.Linq;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium;
using Test.Common;
using Test.Common.Controls;
using OpenQA.Selenium.Support.UI;
using System.Text;

namespace WpfTestApp.UITests.Abstraction
{
    public class MainWindow : WindowBase
    {
        public Button MoveAllButton => GetButton("MoveAllButton");
        public Button StopAllButton => GetButton("StopAllButton");

        public override void CloseWindow()
        {
        }

        public MainWindow(string automationId) : base(automationId)
        {
        }

        private Button GetButton(string automationId) => new Button(this.TryGetElement(automationId));

        public void SelectTarget(int index)
        {
            var targetList = new NameList(this.TryGetElement("TargetSelector"));
            var foundTarget = targetList.GetItems()[index].Element;
            foundTarget.Click();
        }

        public void SelectTarget(string target)
        {
            var targetList = new NameList(this.TryGetElement("TargetSelector"));
            var foundTarget = targetList.GetItems().SingleOrDefault(x => x.Element.Text == target).Element;
            foundTarget.Click();
        }

        public RobotView GetRobotView(int index)
        {
            var robotView = new NameList(this.TryGetElement("RobotList"));
            return new RobotView(robotView.GetItems()[index].Element);
        }
        public RobotView GetRobotView(string robot)
        {
            var robotView = new NameList(this.TryGetElement("RobotList"));
            return new RobotView(robotView.GetItems().SingleOrDefault(x => x.Element.Text == robot).Element);
        }
        

        public IReadOnlyCollection<WindowsElement> GetAllGoButton()
        {
            var goButtonElements = WindowsDriver.FindElementsByAccessibilityId("GoButton");
            
            return goButtonElements;
        }
        public List<string> GetAllRobotStatus()
        {
            List<string> statuses = new List<string>();

            var robotViews = GetAllRobots();

            foreach (var robotView in robotViews)
            {
                string status = robotView.Status;
                statuses.Add(status);
            }

            return statuses;
        }
        
        public List<RobotView> GetAllRobots()
        {
            var robotList = new NameList(this.TryGetElement("RobotList"));
            return robotList.GetItems().Select(item => new RobotView(item.Element)).ToList();
        }
        
        public string GetErrorMessage()
        {
            try
            {
                var wait = new WebDriverWait(WindowsDriver, TimeSpan.FromSeconds(10));
                var listBoxItems = wait.Until(driver => driver.FindElements(By.ClassName("ListBoxItem")));

                StringBuilder messageBuilder = new StringBuilder();

                foreach (var item in listBoxItems)
                {
                    var textBlocks = item.FindElements(By.ClassName("TextBlock"));

                    foreach (var textBlock in textBlocks)
                    {
                        messageBuilder.AppendLine(textBlock.Text);
                    }
                }

                return messageBuilder.ToString().Trim();
            }
            catch(NoSuchElementException ex)
            {
                Console.WriteLine("No Error Message");
                return string.Empty;
            }
        }
        

        public double GetProgressBarStatus()
        {
            try
            {
                var progressBarElement = WindowsDriver.FindElementByClassName("ProgressBar");
                var progressBar = progressBarElement.GetAttribute("RangeValue.Value");
                if (double.TryParse(progressBar, out double result))
                {
                    return result;
                }
                else
                {
                    Console.WriteLine("Failed");
                    return 0.0;
                }
            }
            catch (NoSuchElementException e)
            {
                Console.WriteLine("Unable to retrieve the value : {e.Message}");
            }
            return 0.0;
        }

        public TimeSpan GetTime()
        {
            try
            {
                var timeRemainingElement = WindowsDriver.FindElementByClassName("TextBlock");
                var timeRemainingText = timeRemainingElement.Text;

                if (TimeSpan.TryParse(timeRemainingText, out TimeSpan timeRemaining))
                {
                    return timeRemaining;
                }
                else
                {
                    Console.WriteLine("Failed to parse remaining time.");
                    return TimeSpan.Zero;
                }
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine($"Failed to find remaining time element: {ex.Message}");
                return TimeSpan.Zero;
            }
        }
    }
}
