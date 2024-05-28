using OpenQA.Selenium.Appium.Windows;
using System;
using System.Linq;

namespace Test.Common
{
    public static class ElementExtensions
    {
        private const string XpathFallBackMessage = "Error locating element by Automation Id ({0}). Message: {1} - trying xpath fall back";
        private const string XpathQuery = "*/*[@AutomationId=\"{0}\"]";

        private const string InitialAttemptMessage = "Initial attempt to find element by {0} ({1}) was unsuccessful. Refreshing parent and retrying";

        private const string InitialTestInstanceAttemptMessage = "Initial attempt to find element by {0} ({1}) was unsuccessful. Re-attaching driver and retrying";
        private const string FinalTestInstanceAttemptMessage = "Second attempt to find element by {0} ({1}) was unsuccessful. Killing, re-launching driver and retrying";

        public static Element TryGetElement(this FrameworkElementBase frameworkElement, string automationId, RetryInterval searchTimeOut = RetryInterval.Normal)
        {
            var windowsElement = (WindowsElement)TryGetAppiumElement(() =>
            {
                try
                {
                    return frameworkElement.Element.FindElementByAccessibilityId(automationId);
                }
                catch (Exception)
                {
                    try
                    {
                        Console.WriteLine(InitialAttemptMessage, nameof(automationId), automationId);

                        // Refresh the parent
                        frameworkElement.RefreshElement();
                        return frameworkElement.Element.FindElementByAccessibilityId(automationId);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(XpathFallBackMessage, automationId, e.Message);
                        return frameworkElement.Element.WrappedDriver.FindElement(OpenQA.Selenium.By.XPath(string.Format(XpathQuery, automationId)));
                    }
                }
            }, automationId, searchTimeOut);

            return Element.Create(windowsElement, automationId, null, null, frameworkElement);
        }

        public static Element TryGetElementByXPath(this FrameworkElementBase frameworkElement, string automationId, RetryInterval searchTimeOut = RetryInterval.Normal)
        {
            var windowsElement = (WindowsElement)TryGetAppiumElement(() =>
            {
                try
                {
                    return frameworkElement.Element.FindElementByXPath(automationId);
                }
                catch (Exception)
                {
                    try
                    {
                        Console.WriteLine(InitialAttemptMessage, nameof(automationId), automationId);

                        // Refresh the parent
                        frameworkElement.RefreshElement();
                        return frameworkElement.Element.FindElementByAccessibilityId(automationId);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(XpathFallBackMessage, automationId, e.Message);
                        return frameworkElement.Element.WrappedDriver.FindElement(OpenQA.Selenium.By.XPath(string.Format(XpathQuery, automationId)));
                    }
                }
            }, automationId, searchTimeOut);

            return Element.Create(windowsElement, automationId, null, null, frameworkElement);
        }
        public static Element TryGetElementByName(this FrameworkElementBase frameworkElement, string name, RetryInterval searchTimeOut = RetryInterval.Normal)
        {
            var windowsElement = TryGetAppiumElement(() =>
            {
                try
                {
                    return (WindowsElement)frameworkElement.Element.FindElementByName(name);
                }
                catch (Exception)
                {
                    Console.WriteLine(InitialAttemptMessage, nameof(name), name);

                    // Refresh the parent
                    frameworkElement.RefreshElement();
                    return (WindowsElement)frameworkElement.Element.FindElementByName(name);
                }

            }, name, searchTimeOut);

            return Element.Create(windowsElement, null, name, null, frameworkElement);
        }

        public static Element[] TryGetElementsByClass(this FrameworkElementBase frameworkElement, string className, RetryInterval searchTimeOut = RetryInterval.Normal)
        {
            var windowsElements = TryGetAppiumElement(() =>
            {
                try
                {
                    return frameworkElement.Element.FindElementsByClassName(className).ToArray();
                }
                catch (Exception)
                {
                    Console.WriteLine(InitialAttemptMessage, nameof(className), className);

                    // Refresh the parent
                    frameworkElement.RefreshElement();
                    return frameworkElement.Element.FindElementsByClassName(className).ToArray();
                }

            }, className, searchTimeOut).Cast<WindowsElement>();

            return windowsElements.Select(x => Element.Create(x, null, null, null, frameworkElement)).ToArray();
        }

        public static Element TryGetElementByClass(this FrameworkElementBase frameworkElement, string className, RetryInterval searchTimeOut = RetryInterval.Normal)
        {
            var windowsElement = (WindowsElement)TryGetAppiumElement(() =>
            {
                try
                {
                    return frameworkElement.Element.FindElementByClassName(className);
                }
                catch (Exception)
                {
                    Console.WriteLine(InitialAttemptMessage, nameof(className), className);

                    // Refresh the parent
                    frameworkElement.RefreshElement();
                    return frameworkElement.Element.FindElementByClassName(className);
                }

            }, className, searchTimeOut);

            return Element.Create(windowsElement, null, null, className, frameworkElement);
        }

        public static Element TryGetElementByXPaths(this FrameworkElementBase frameworkElement, string xPath, RetryInterval searchTimeOut = RetryInterval.Normal)
        {
            WindowsElement windowsElement = (WindowsElement)TryGetAppiumElement(delegate
            {
                try
                {
                    return frameworkElement.Element.FindElementByXPath(xPath);
                }
                catch (Exception)
                {
                    Console.WriteLine("Initial attempt to find element by {0} ({1}) failed. Refreshing parent and retrying", "xPath", xPath);
                    frameworkElement.RefreshElement();
                    return frameworkElement.Element.FindElementByXPath(xPath);
                }
            }, xPath, searchTimeOut);

            return Element.Create(windowsElement, windowsElement.Id, null, null, frameworkElement);
        }

        public static Element TryGetElement(this WindowsDriver<WindowsElement> element, string automationId, RetryInterval searchTimeOut = RetryInterval.Normal)
        {
            var windowsElement = TryGetAppiumElement(() => element.FindElementByAccessibilityId(automationId), automationId, searchTimeOut);

            return Element.Create(windowsElement, automationId);
        }

        public static Element TryGetElement(this Element element, string automationId, RetryInterval searchTimeOut = RetryInterval.Normal)
        {
            var windowsElement = (WindowsElement)TryGetAppiumElement(() => element.FindElementByAccessibilityId(automationId), automationId, searchTimeOut);

            return Element.Create(windowsElement, automationId);
        }

        public static Element TryGetElementByClassName(this WindowsDriver<WindowsElement> element, string className, RetryInterval searchTimeOut = RetryInterval.Normal)
        {
            var windowsElement = TryGetAppiumElement(() => element.FindElementByClassName(className), className, searchTimeOut);

            return Element.Create(windowsElement, null, null, className);
        }

        public static Element TryGetElement(string automationId, RetryInterval searchTimeOut = RetryInterval.Normal)
        {
            var windowsElement = TryGetAppiumElement(() =>
            {
                try
                {
                    return TestInstance.Instance.WindowsDriver.FindElementByAccessibilityId(automationId);
                }
                catch (Exception)
                {
                    // Second attempt, now reattach appium to the application and try again
                    try
                    {
                        Console.WriteLine(InitialTestInstanceAttemptMessage, nameof(automationId), automationId);
                        TestInstance.Instance.AttachWinAppDriverToApplication(TestInstance.Instance.CurrentWindow.Element.AutomationId);
                        return TestInstance.Instance.WindowsDriver.FindElementByAccessibilityId(automationId);
                    }
                    catch (Exception)
                    {
                        // Final attempt. Kill appium, reattach and try again
                        Console.WriteLine(FinalTestInstanceAttemptMessage, nameof(automationId), automationId);
                        ProcessManagement.KillAndLaunchAppium();
                        TestInstance.Instance.AttachWinAppDriverToApplication(TestInstance.Instance.CurrentWindow.Element.AutomationId);
                        return TestInstance.Instance.WindowsDriver.FindElementByAccessibilityId(automationId);
                    }
                }
            }, automationId, searchTimeOut);

            return Element.Create(windowsElement, automationId);
        }

        public static Element[] TryGetElements(this FrameworkElementBase frameworkElement, string automationId, RetryInterval searchTimeOut = RetryInterval.Normal)
        {
            var windowsElements = TryGetAppiumElement(() =>
            {
                try
                {
                    return frameworkElement.Element.FindElementsByAccessibilityId(automationId).ToArray();
                }
                catch (Exception)
                {
                    try
                    {
                        Console.WriteLine(InitialAttemptMessage, nameof(automationId), automationId);

                        // Refresh the parent
                        frameworkElement.RefreshElement();
                        return frameworkElement.Element.FindElementsByAccessibilityId(automationId).ToArray();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(XpathFallBackMessage, automationId, e.Message);
                        return (WindowsElement[])frameworkElement.Element.WrappedDriver.FindElements(OpenQA.Selenium.By.XPath(string.Format(XpathQuery, automationId))).ToArray();
                    }
                }
            }, automationId, searchTimeOut).Cast<WindowsElement>();

            return windowsElements.Select(x => Element.Create(x, null, null, null, frameworkElement)).ToArray();
        }

        private static T TryGetAppiumElement<T>(Func<T> element, string id, RetryInterval searchTimeOut = RetryInterval.Normal)
        {
            var errorText = string.Empty;

            object windowsElement = null;

            if (searchTimeOut != RetryInterval.None)
            {
                FunctionRunner.RunFuncUntilSuccess(() =>
                {
                    try
                    {
                        windowsElement = element();
                        return true;
                    }
                    catch (Exception e)
                    {
                        errorText = "Error locating windows element:" + id + " within the time specified {0}. Stack trace: " + e.StackTrace;
                        return false;
                    }
                }, () => errorText, ToRetryTimeOut(searchTimeOut));
            }
            else
            {
                windowsElement = element();
            }

            return (T)windowsElement;
        }

        private static int ToRetryTimeOut(RetryInterval retryInterval)
        {
            return ((int)retryInterval);
        }

        public static bool IsPlaceHolder(this WindowsElement element)
        {
            return element.GetAttribute("AcceleratorKey") == "True";
        }
    }

    public enum RetryInterval
    {
        None = 0,
        Normal = 2000,
        Greater = 5000,
        Extended = 10000,
        SuperExtended = 30000,
    }
}