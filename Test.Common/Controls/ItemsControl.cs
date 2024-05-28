using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Test.Common.Controls
{
    public abstract class ItemsControl<T> : ItemsControl where T : WindowsElementBase
    {
        protected ItemsControl(Element element, string elementAutomationId) : base(element, elementAutomationId) { }
        protected abstract T CreateElement(Element element);

        public T GetItemByName(string name)
        {
            return CreateElement(GetFirstByName(name));
        }

        public List<T> GetItems()
        {
            return GetListItems().Select(CreateElement).ToList();
        }

        public new T GetLastItem()
        {
            return CreateElement(base.GetLastItem());
        }
    }

    public class ItemsControl : WindowsElementBase
    {
        protected readonly string ChildElementAutomationId;

        protected ItemsControl(Element element, string childElementAutomationId) : base(element)
        {
            ChildElementAutomationId = childElementAutomationId;
        }

        protected Element[] ItemCache => this.TryGetElements(ChildElementAutomationId);

        public int ItemCount()
        {
            return ItemCache.Length;
        }

        protected virtual IEnumerable<Element> GetListItems()
        {
            return ItemCache;
        }

        protected Element GetFirstItem()
        {
            return ItemCache.FirstOrDefault();
        }

        protected Element GetLastItem()
        {
            return ItemCache.LastOrDefault();
        }

        protected Element GetByIndex(int index)
        {
            return ItemCache[index];
        }

        /// <summary>
        /// Using GetAttribute here instead of Text property so that elements wrapped in AutomationGrid
        /// do not cause the UI to scroll up and down as the list is queried
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected IEnumerable<Element> GetByName(string name)
        {
            return ItemCache.Where(x => x.GetAttribute("Name").Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Using GetAttribute here instead of Text property so that elements wrapped in AutomationGrid
        /// do not cause the UI to scroll up and down as the list is queried
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Element GetFirstByName(string name)
        {
            return ItemCache.FirstOrDefault(x => x.GetAttribute("Name").Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool IsVerticallyScrollable
        {
            get
            {
                bool.TryParse(Element.GetAttribute("Scroll.VerticallyScrollable"), out var state);

                return state;
            }
        }

        public float VerticalScrollOffset
        {
            get
            {
                float.TryParse(Element.GetAttribute("Scroll.VerticalScrollPercent"), out var position);

                return position;
            }
        }

        public bool IsHorizontallyScrollable
        {
            get
            {
                bool.TryParse(Element.GetAttribute("Scroll.HorizontallyScrollable"), out var state);

                return state;
            }
        }

        public float HorizontalScrollOffset
        {
            get
            {
                float.TryParse(Element.GetAttribute("Scroll.HorizontalScrollPercent"), out var position);

                return position;
            }
        }

        public void PageUp()
        {
            Element.SendKeys(Keys.PageUp);
        }

        public void PageDown()
        {
            Element.SendKeys(Keys.PageDown);
        }

        public void ScrollToTop()
        {
            Element.SendKeys(Keys.Control + Keys.Home + Keys.Control);
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            WaitForVerticalScrollOffset(f => f == 0.00f);
        }

        public void ScrollToBottom()
        {
            Element.SendKeys(Keys.Control + Keys.End + Keys.Control);
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            WaitForVerticalScrollOffset(f => f == 100.00f);
        }

        /// <summary>
        /// Wait until vertical scroll offset is satisfied by the predicate or times out
        /// </summary>
        /// <param name="predicate">return true if VerticalOffset meets condition</param>
        public void WaitForVerticalScrollOffset(Predicate<float> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (!IsVerticallyScrollable) return;

            var initialOffset = VerticalScrollOffset;

            // Exit if scroll offset already met
            if (predicate(initialOffset))
            {
                return;
            }

            FunctionRunner.RunFuncUntilSuccess(() =>
            {
                try
                {

                    return predicate(VerticalScrollOffset);
                }
                catch (Exception)
                {
                    // ignored as exception will be thrown by FunctionRunner.RunFuncUntilSuccess 
                }

                return false;

            }, () => "ItemsControl - WaitForScroll - condition not reached within time {0}", 5000);

        }

        public virtual void WaitForNoOfItems(int noOfItems)
        {
            FunctionRunner.RunFuncUntilSuccess(() =>
            {
                try
                {
                    if (GetListItems().Count() == noOfItems) return true;
                }
                catch (Exception)
                {
                    // ignored as exception will be thrown by FunctionRunner.RunFuncUntilSuccess 
                }

                RefreshElement();

                return false;

            }, () => $"ItemsControl - WaitForNoOfItems - Qty: {noOfItems} not found within time {{0}}", 15000);
        }

        public virtual void WaitForMinNoOfItems(int noOfItems)
        {
            FunctionRunner.RunFuncUntilSuccess(() =>
            {
                try
                {
                    if (GetListItems().Count() >= noOfItems) return true;
                }
                catch (Exception)
                {
                    // ignored as exception will be thrown by FunctionRunner.RunFuncUntilSuccess 
                }

                RefreshElement();

                return false;

            }, () => $"ItemsControl - WaitForMinNoOfItems - Qty: {noOfItems} not found within time {{0}}", 30000);
        }
    }
}