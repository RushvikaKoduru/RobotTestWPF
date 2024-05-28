

using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Test.Common.Controls;


namespace Test.Common
{
    public abstract class WindowsElementBase : FrameworkElementBase
    {
        public WindowsElementBase() { }
        
        protected WindowsElementBase(Element element) : base(element)
        {
        }
        
        public virtual bool IsSelected => Element.Selected;

        public virtual void Click()
        {
            Element.Click();
        }

        /// <summary>
        /// This can be very slow. So use sparingly!
        /// </summary>
        /// <returns></returns>
        public Element Parent()
        {
            var parentXPath = $".//*[@RuntimeId='{Element.Id}']/parent::*";

            return Element.Create(WindowsDriver.FindElementByXPath(parentXPath));
        }

        public void LongPress()
        {
            var elementAction = new Actions(WindowsDriver);
            elementAction.ClickAndHold(Element);
            elementAction.Perform();
        }

        public void MouseDown()
        {
            WindowsDriver.Mouse.MouseMove(Element.Coordinates);
            WindowsDriver.Mouse.MouseDown(null);
        }

        public Task DragMouseForDuration(WindowsElement targetElement, int duration)
        {
            Element.Click();
            WindowsDriver.Mouse.MouseDown(null);
            WindowsDriver.Mouse.MouseMove(targetElement.Coordinates);

            return Task.Run(() =>
            {
                Thread.Sleep(duration);

                MouseUp();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offsetX">X position to move mouse to relative to centre of current element</param>
        /// <param name="offsetY">Y position to move mouse to relative to centre of current element</param>
        /// <param name="continueMove">Func used to determine when to stop dragging</param>
        /// <param name="timeToWait">overall timeout</param>
        public void DragMouseUntilCondition(int offsetX, int offsetY, Func<bool> continueMove, int timeToWait = 30000)
        {
            Element.Click();
            WindowsDriver.Mouse.MouseDown(null);
            WindowsDriver.Mouse.MouseMove(null, offsetX, offsetY);

            FunctionRunner.RunFuncUntilSuccess(continueMove, () => "Failed to drag mouse until condition met", timeToWait, 100); ;
            MouseUp();
        }

        public Actions DragToOffset(int x, int y)
        {
            var action = new Actions(WindowsDriver);
            action.ClickAndHold(Element).MoveByOffset(x, y).Build().Perform();
            return action;
        }

        public Actions MoveMouseToElement(int offsetX, int offsetY)
        {
            var action = new Actions(WindowsDriver);
            action.MoveToElement(Element, offsetX, offsetY).Perform();
            return action;
        }

        public void DragAndDropAtOffset(int x, int y)
        {
            var action = DragToOffset(x, y);
            action.Release().Perform();
        }

        public void DragAndDropOnElement(WindowsElement destination)
        {
            var offSetX = (destination.Location.X + destination.Size.Width / 2) - Element.Location.X;
            var offsetY = (destination.Location.Y + destination.Size.Height / 2) - Element.Location.Y;
            DragAndDropAtOffset(offSetX, offsetY);
        }

        public void MouseUp()
        {
            WindowsDriver.Mouse.MouseUp(null);
        }

        public virtual Size GetSize => Element.Size;

        public virtual Point GetLocation => Element.Location;

        public bool IsOffScreen => Element.GetAttribute("IsOffscreen") == "True";

        public bool IsEnabled => Element.GetAttribute("IsEnabled") == "True";

        public void Move(int x, int y, bool verifyWithWindowCoordinates = false, int timeToWait = 60000)
        {
            WaitForFunc((() =>
                {
                    var initialPosition = verifyWithWindowCoordinates ? WindowsDriver.Manage().Window.Position : Element.Location;

                    var elementAction = new Actions(WindowsDriver);
                    elementAction.DragAndDropToOffset(Element, x, y);
                    elementAction.Perform();

                    var newPosition = verifyWithWindowCoordinates ? WindowsDriver.Manage().Window.Position : Element.Location;

                    return newPosition != initialPosition;
                })
                , "Failed to detect movement within the expected time ({0} milliseconds)"
                , timeToWait);
        }

        public void TabAndSelectItemWithSpace()
        {
            TabToElement(Element);
            WindowsDriver.Keyboard.PressKey(Keys.Space);
        }

        public void TabAndSelectItemWithReturn()
        {
            TabToElement(Element);
            WindowsDriver.Keyboard.PressKey(Keys.Enter);
        }

        public void Escape()
        {
            WindowsDriver.Keyboard.SendKeys(Keys.Escape);
        }

        public bool IsDisplayed()
        {
            try
            {
                return Element.Displayed;
            }
            catch (Exception e)
            {
                Console.WriteLine("Element IsDisplayed failed {0}", e.Message);
                return false;
            }
        }

        public bool IsNotDisplayed()
        {
            try
            {
                WaitForFunc(() => Element.Displayed == false,
                    "Failed to detect displayed state false within the expected time ({0} milliseconds)");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Safe check for when an element has never been displayed 
        /// and therefore the page source contains no reference to it
        /// Search is done from the root (mainWindow)
        /// </summary>
        /// <returns></returns>
        public bool ElementAvailable(string elementName)
        {
            try
            {
                WindowsDriver.TryGetElement(elementName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Safe check for when an element not being displayed, avoiding all the retry logic trying to find the element
        /// If the item doesn't exist and has never been shown then we have a quick exit. If the item has been shown
        /// then we have to poll it if its visible just in case it hasn't quite been hidden yet.
        /// This function is safer and quicker than getting the element and asking if it is displayed.
        /// </summary>
        /// <returns>True if the element does not exist</returns>
        public bool ElementNotDisplayed(string elementName)
        {
            bool visible = false;
            try
            {
                var element = WindowsDriver.TryGetElement(elementName);
                FunctionRunner.RunFuncUntilSuccess(() =>
                {
                    // try to get the elements display property, if it fails an exception will be raised 
                    // which means it is not displayed / available
                    visible = element.Displayed;

                    // if the elements display property is available and the element is displayed then return true which will also throw an exception
                    return !visible;

                }, () => $"Checking element {elementName} is not visible, but it is!", 10000);
            }
            catch (Exception)
            {
                // ignored
            }

            return !visible;
        }

        /// <summary>
        /// Safe check for when an element has never been displayed 
        /// and therefore the page source contains no reference to it
        /// Search is done relative to the current element
        /// </summary>
        /// <returns></returns>
        public bool ChildElementAvailable(string elementName)
        {
            try
            {
                Element.TryGetElement(elementName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected void WaitForFunc(Func<bool> condition, string message, int? timeToWait = null)
        {
            FunctionRunner.RunFuncUntilSuccess(condition, () => message, timeToWait);
        }

        /// <summary>
        /// Finds an attribute from the page source
        /// This can be slow, so use sparingly
        /// </summary>
        /// <returns></returns>
        protected string GetAttributeFromPageSource(string root, string attribute, bool searchDescendants)
        {
            var runTimeId = "RuntimeId";

            var pageElements = XElement.Parse(WindowsDriver.PageSource);

            XAttribute property = null;

            var xElement = searchDescendants
                ? pageElements.Descendants(root).Attributes().Where(x => x.Name == runTimeId).First(y => y.Value == Element.Id).Parent
                : pageElements.Attributes().Where(x => x.Name == runTimeId).First(y => y.Value == Element.Id).Parent;

            if (xElement != null)
                property = xElement.Attributes().FirstOrDefault(z => z.Name == attribute);

            if (property == null)
            {
                throw new Exception("Could not find xml element from page source");
            }

            return property.Value;
        }

        protected void TabToElement(WindowsElement element)
        {
            WaitForFunc(() =>
            {
                var focusedElementId = ((WindowsElement)WindowsDriver.SwitchTo().ActiveElement()).Id;

                if (focusedElementId != element.Id)
                {
                    WindowsDriver.Keyboard.PressKey(Keys.Tab);
                    return false;
                }

                return true;
            }, "Failed to tab to element within the expected time ({0} milliseconds)", 60000);
        }

        public bool HasFocus()
        {
            var focusedElementId = ((WindowsElement)WindowsDriver.SwitchTo().ActiveElement()).Id;

            return Element.Id == focusedElementId;
        }

        public void WaitForElementEnabledState(bool state)
        {
            FunctionRunner.RunFuncUntilSuccess(() =>
            {
                try
                {
                    if (IsEnabled == state) return true;
                }
                catch (Exception)
                {
                    // ignored
                }

                Console.WriteLine("Button enabled state not detected retrying...");
                RefreshElement();
                return false;

            }, () => "WindowsElementBase - WaitForElementEnabled - Failed to detect element enabled state: " + state + " " + Element.AutomationId + " within {0}", 30000);
        }

        public void Copy()
        {
            Element.Click();
            WindowsDriver.Keyboard.PressKey(Keys.Control);
            WindowsDriver.Keyboard.PressKey("a");
            WindowsDriver.Keyboard.PressKey("c");
            WindowsDriver.Keyboard.ReleaseKey(Keys.Control);
        }

        public void Paste()
        {
            Element.Click();
            WindowsDriver.Keyboard.PressKey(Keys.Control);
            WindowsDriver.Keyboard.PressKey("a");
            WindowsDriver.Keyboard.PressKey("v");
            WindowsDriver.Keyboard.ReleaseKey(Keys.Control);

            Console.WriteLine($"Pasted text: {Element.Text}");
        }

        
    }
}