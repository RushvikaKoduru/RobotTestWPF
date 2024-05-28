using OpenQA.Selenium.Appium.Windows;
using System;
using System.Runtime.CompilerServices;


namespace Test.Common
{
    /// <summary>
    /// Base class for UI elements which may or may not be part of the visual tree
    /// </summary>
    public abstract class FrameworkElementBase
    {
        public FrameworkElementBase() { }

        protected FrameworkElementBase(Element element)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Initialize(element);
        }

        public virtual void Initialize(Element element)
        {
            Element = element;
        }

        public WindowsDriver<WindowsElement> WindowsDriver => TestInstance.Instance.WindowsDriver;

        /// <summary>
        /// Provides robust access to elements in the visual tree
        /// </summary>
        public Element Element { get; private set; }

        public string AcceleratorKey => Element.GetAttribute("AcceleratorKey");
        public string HelpText => Element.GetAttribute("HelpText");

        /// <summary>
        /// Allow an element to refresh itself
        /// </summary>
        public void RefreshElement()
        {
            Element = TestInstance.Instance.WindowsDriver.TryGetElement(Element.AutomationId, RetryInterval.SuperExtended);
        }

        protected TElement Get<TElement>([CallerMemberName] string automationId = null) where TElement : FrameworkElementBase, new()
        {
            return Get<TElement>(fromPopup: false, automationId);
        }

        protected TElement Get<TElement>(bool fromPopup, [CallerMemberName] string automationId = null) where TElement : FrameworkElementBase, new()
        {
            try
            {
                var element = ((!fromPopup) ? this.TryGetElement(automationId) : TestInstance.Instance.WindowsDriver.TryGetElement(automationId));
                return WrapElement<TElement>(element);
            }
            catch (TimeoutException)
            {
                return null;
            }
        }

        protected TElement WrapElement<TElement>(Element element) where TElement : FrameworkElementBase, new()
        {
            var val = new TElement();
            val.Initialize(element);
            return val;
        }
    }
}