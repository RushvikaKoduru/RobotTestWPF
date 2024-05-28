using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Remote;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Test.Common
{
    /// <summary>
    /// Windows element wrapper to provide robust access to elements.
    /// Allows the tests to automatically re-initialise the winapp driver if in
    /// an error state 
    /// </summary>
    public class Element : WindowsElement
    {
        // ParentFrameworkElement will be null for top level windows and some operations where an element is searched on the fly
        private FrameworkElementBase _parentFrameworkElement;

        // The element is not unique if one of multiple elements with the same id, this would be true for list items
        private bool _isUnique;

        private WindowsElement _elementProxy;

        public string AutomationId { get; private set; }
        private string Name { get; set; }
        private string ClassName { get; set; }

        public Element(RemoteWebDriver parent, string id) : base(parent, id)
        {
            _elementProxy = this;
        }

        public static Element Create(WindowsElement windowsElement, string automationId = null, string name = null, string className = null, FrameworkElementBase parentFrameworkElement = null)
        {
            var testElement = new Element((WindowsDriver<WindowsElement>)windowsElement.WrappedDriver, windowsElement.Id)
            {
                Name = name,
                ClassName = className,
                AutomationId = automationId,
                _parentFrameworkElement = parentFrameworkElement,
                _isUnique = !(automationId == null && name == null && className == null)
            };

            return testElement;
        }

        public new string Text => TryElement(() =>
        {
            return _elementProxy.Text;
        });

        public new string GetAttribute(string attributeName) => TryElement(() =>
        {
            return _elementProxy.GetAttribute(attributeName);
        });

        public new bool Selected => TryElement(() =>
        {
            return _elementProxy.Selected;
        });

        public new bool Displayed => TryElement(() => _elementProxy.Displayed);

        public new void Click() => TryElement<object>(() =>
        {
            _elementProxy.Click();
            return null;
        });

        public new string Id => TryElement(() => _elementProxy.Id);
        public new ICoordinates Coordinates => TryElement(() => _elementProxy.Coordinates);
        public new Point Location => TryElement(() => _elementProxy.Location);
        public new Size Size => TryElement(() => _elementProxy.Size);

        public void SendKeys(string keys, [CallerMemberName] string caller = "") => TryElement<object>(() =>
        {
            _elementProxy.SendKeys(keys);
            return null;
        });

        public new void Clear() => TryElement<object>(() =>
        {
            _elementProxy.Clear();
            return null;
        });

        public new AppiumWebElement FindElementByXPath(string xpath) => TryElement(() => _elementProxy.FindElementByXPath(xpath));

        private T TryElement<T>(Func<T> elementAction)
        {
            try
            {
                return elementAction();
            }
            catch (Exception e)
            {
                if (_isUnique && _parentFrameworkElement != null)
                {
                    if (AutomationId != null)
                    {
                        _elementProxy = _parentFrameworkElement.TryGetElement(AutomationId);
                        return elementAction();
                    }

                    if (Name != null)
                    {
                        _elementProxy = _parentFrameworkElement.TryGetElementByName(Name);
                        return elementAction();
                    }

                    if (ClassName != null)
                    {
                        _elementProxy = _parentFrameworkElement.TryGetElementByClass(Name);
                        return elementAction();
                    }
                }

                // Non unique elements cannot be recovered when an error occurs. Although we know the id of the parent container (list)
                // We do not know which list item is being interacted with.

                // ReSharper disable once PossibleIntendedRethrow
                throw e;
            }
        }
    }
}