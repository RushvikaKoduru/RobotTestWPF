using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using Test.Common;
using WpfTestApp.UITests.Abstraction;

namespace WpfTestApp.UITests
{
    [TestClass]
    public class MoveAndStopTests
    {
        private MainWindow _mainWindow;
        private const string AppName = "Head Test Utility";
        //private const string ExecutablePath = @"C:\Temp\test\SDET Test\WpfTestApp\bin\Debug\TestApp.exe";
        private const string ExecutablePath = @"C:\Users\rushv\OneDrive\Desktop\Programs\SDET Test\SDET Test\WpfTestApp\bin\Debug\TestApp.exe";

        [TestInitialize]
        public void Setup()
        {
            TestInstance.Instance.Initialise(AppName, ExecutablePath);
            var windows = new Windows();
            _mainWindow = windows.MainWindow;
        }

        [TestCleanup]
        public void CleanUp()
        {
            TestInstance.Instance.CloseApp();
        }

        
        [TestMethod]
        public void MoveRobot()
        {
            _mainWindow.SelectTarget("Target 4");
            var robotView = _mainWindow.GetRobotView("Robot #5");
            robotView.GoButton.Click();
            Assert.IsFalse(robotView.GoButton.IsEnabled);
            Assert.AreEqual("Moving", robotView.Status);

            //double progressBar = _mainWindow.GetProgressBarStatus();
            //Thread.Sleep(1000);
            //double updateProgressBar = _mainWindow.GetProgressBarStatus();
            //Assert.IsTrue(updateProgressBar > progressBar);

            //TimeSpan initialTimeToShot = _mainWindow.GetTime();
            //Thread.Sleep(1000);
            //TimeSpan updatedTimeToShot = _mainWindow.GetTime();
            //Assert.IsTrue(updatedTimeToShot < initialTimeToShot);

        }

        [TestMethod]
        public void MoveAllRobots()
        {
            _mainWindow.SelectTarget("Target 1");
            _mainWindow.MoveAllButton.Click();

            var goButtons = _mainWindow.GetAllGoButton();
            int i = 0;
            foreach (WindowsElement button in goButtons)
            {
                if (i != 5)
                {
                    Assert.IsFalse(button.Enabled, "Go button should be disabled after moving all robots");
                }

                i++;
            }

            Assert.IsTrue(_mainWindow.StopAllButton.IsEnabled);
        }
        [TestMethod]
        public void StopRobot()
        {
            _mainWindow.SelectTarget("Target 1");
            var robotView = _mainWindow.GetRobotView("Robot #1");
            robotView.GoButton.Click();
            Thread.Sleep(1000);

            robotView.StopButton.Click();
            Thread.Sleep(1000);
            Assert.IsTrue(robotView.GoButton.IsEnabled);
            Assert.AreEqual("Idle", robotView.Status);
        }

        [TestMethod]
        public void StopAllRobots()
        {
            _mainWindow.SelectTarget("Target 1");
            _mainWindow.MoveAllButton.Click();
            Thread.Sleep(1000);
            _mainWindow.StopAllButton.Click();
            Thread.Sleep(1000);

            var goButtons = _mainWindow.GetAllGoButton();

            foreach (WindowsElement button in goButtons)
            {
                Assert.IsTrue(button.Enabled);
            }
            var robotViews = _mainWindow.GetAllRobotStatus();
            foreach (var status in robotViews)
            {
                Assert.AreEqual("Idle", status);
            }


        }


        [TestMethod]
        public void DuplicateTargetMove()
        {
            _mainWindow.SelectTarget("Target 1");
            var robotView = _mainWindow.GetRobotView("Robot #2");
            robotView.GoButton.Click();
            Assert.IsFalse(robotView.GoButton.IsEnabled);

            Thread.Sleep(5000);
            robotView.GoButton.Click();
            Assert.IsTrue(robotView.GoButton.IsEnabled);
            Assert.AreEqual("Idle", robotView.Status);

        }

        [TestMethod]
        public void DifferentTarget()
        {
            _mainWindow.SelectTarget("Target 2");
            var robotView1 = _mainWindow.GetRobotView("Robot #1");
            robotView1.GoButton.Click(); //Click first robot with Target 1
            Assert.IsFalse(robotView1.GoButton.IsEnabled);
            Assert.AreEqual("Moving", robotView1.Status);

            Thread.Sleep(1000);
            _mainWindow.SelectTarget("Target 3");
            robotView1.GoButton.Click(); // Click same robot with Target 2
            Assert.AreEqual("Moving", robotView1.Status);
            Assert.IsFalse(robotView1.GoButton.IsEnabled);
        }
        [TestMethod]
        public void ErrorMessageOfRobots()
        {
            _mainWindow.SelectTarget("Target 1");
            
            _mainWindow.MoveAllButton.Click();

            Thread.Sleep(2000);

            var messageText = _mainWindow.GetErrorMessage();
            Assert.IsTrue(!string.IsNullOrEmpty(messageText));
            Assert.IsTrue(messageText.Contains("Robot"));
     
        }

    }
}