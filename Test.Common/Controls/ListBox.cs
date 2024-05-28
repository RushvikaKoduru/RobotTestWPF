using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Test.Common.Controls
{
    public abstract class ListBox : ItemsControl
    {
        protected ListBox(Element element, string elementAutomationId) : base(element, elementAutomationId)
        {
        }
    }

    public class ListBox<T> : ListBox where T : WindowsElementBase
    {
        public ListBox(Element element, string elementAutomationId) : base(element, elementAutomationId) { }
        public ListBox(Element element) : base(element, "ListBoxItem") { }

        public new int ItemCount()
        {
            var elementCount = 0;

            FunctionRunner.RunFuncUntilSuccess(() =>
            {
                try
                {
                    ScrollToTop();

                    if (!IsVirtualisedList || IsVirtualisedList && !IsVerticallyScrollable)
                    {
                        elementCount = base.ItemCount();
                        return true;
                    }

                    var tempElementCache = CacheElements();

                    ScrollToTop();

                    elementCount = tempElementCache.Count;

                    return true;

                }
                catch (Exception)
                {
                    RefreshElement();
                    return false;
                }

            }, () => "Listbox - ItemCount - Unable to query item count within {0}", 120000);

            return elementCount;
        }

        public new T GetFirstItem()
        {
            object createdElement = null;

            FunctionRunner.RunFuncUntilSuccess(() =>
            {
                try
                {

                    if (IsVirtualisedList && IsVerticallyScrollable) ScrollToTop();

                    createdElement = RefreshAndCreateElement(base.GetFirstItem());

                    return true;
                }
                catch (Exception)
                {
                    RefreshElement();
                    return false;
                }

            }, () => "Listbox - GetFirstItem - Unable to get first item " + typeof(T) + " within {0}", 30000);

            return (T)createdElement;
        }

        public new T GetLastItem()
        {
            object createdElement = null;

            FunctionRunner.RunFuncUntilSuccess(() =>
            {
                try
                {
                    if (IsVirtualisedList && IsVerticallyScrollable) ScrollToBottom();

                    createdElement = RefreshAndCreateElement(base.GetLastItem());

                    return true;
                }
                catch (Exception)
                {
                    RefreshElement();
                    return false;
                }

            }, () => "Listbox - GetLastItem - Unable to get last item " + typeof(T) + " within {0}", 30000);

            return (T)createdElement;
        }

        public T GetItemByIndex(int index)
        {
            object createdElement = null;

            FunctionRunner.RunFuncUntilSuccess(() =>
            {
                try
                {
                    if (IsVirtualisedList && IsVerticallyScrollable) ScrollToTop();

                    if (!IsVirtualisedList || IsVirtualisedList && !IsVerticallyScrollable || index == 0)
                    {
                        createdElement = RefreshAndCreateElement(GetByIndex(index));
                        return true;
                    }

                    var tempElementCache = CacheElements();

                    createdElement = RefreshAndCreateElement(tempElementCache[index]);

                    return true;
                }
                catch (Exception)
                {
                    RefreshElement();
                    return false;
                }

            }, () => "Listbox - GetItemByIndex - Unable to get item by index " + index + " " + typeof(T) + " within {0}", 30000);

            return (T)createdElement;
        }

        private List<Element> CacheElements()
        {
            var tempElementCache = new List<Element>();

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            while (VerticalScrollOffset != 100.00f)
            {
                CheckAndAddItemsToCache(tempElementCache);

                PageDown();
            }

            CheckAndAddItemsToCache(tempElementCache);

            return tempElementCache;
        }

        private void CheckAndAddItemsToCache(List<Element> tempElementCache)
        {
            foreach (var element in ItemCache)
            {
                if (!tempElementCache.Contains(element))
                {
                    tempElementCache.Add(element);
                }
            }
        }

        public T GetItemByName(string name)
        {
            object createdElement = null;

            FunctionRunner.RunFuncUntilSuccess(() =>
            {
                try
                {
                    //TestContext.Progress.WriteLine($"Listbox - GetItemByName - Getting item {name}");

                    if (!IsVirtualisedList || IsVirtualisedList && !IsVerticallyScrollable)
                    {
                        createdElement = RefreshAndCreateElement(GetFirstByName(name));
                        return true;
                    }

                    var element = GetFirstByName(name);

                    do
                    {
                        if (element == null)
                        {
                            var currentOffset = VerticalScrollOffset;
                            while (VerticalScrollOffset != 100.00f && currentOffset == VerticalScrollOffset)
                            {
                                PageDown();
                            }

                            element = GetFirstByName(name);
                        }
                    } while (VerticalScrollOffset != 100.00f && element == null);

                    if (element == null)
                    {
                        ScrollToTop();
                        throw new Exception("Not found re-trying...");
                    }

                    createdElement = RefreshAndCreateElement(element);

                    return true;
                }
                catch (Exception)
                {
                    RefreshElement();
                    return false;
                }
            }, () => "Listbox - GetItemByName - Unable to get item " + typeof(T) + " within {0})", 30000);

            return (T)createdElement;
        }

        /// <summary>
        /// If virtualised will scroll down the list each time it is called to gather up items with the required name
        /// does this as we don't know how many items we are looking for
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<T> GetItemsByName(string name)
        {
            IEnumerable<T> createdElement = null;

            FunctionRunner.RunFuncUntilSuccess(() =>
            {
                try
                {
                    //TestContext.Progress.WriteLine($"Listbox - GetItemByName - Getting item {name}");

                    if (!IsVirtualisedList || IsVirtualisedList && !IsVerticallyScrollable)
                    {
                        createdElement = GetByName(name).Select(RefreshAndCreateElement).ToList();
                        return true;
                    }

                    ScrollToTop();

                    var element = new List<Element>();
                    element.AddRange(GetByName(name));

                    do
                    {
                        var currentOffset = VerticalScrollOffset;

                        while (VerticalScrollOffset != 100.00f && currentOffset == VerticalScrollOffset)
                        {
                            PageDown();
                        }

                        element.AddRange(GetByName(name));
                    } while (VerticalScrollOffset != 100.00f);

                    if (element.Count == 0)
                    {
                        throw new Exception("Not found re-trying...");
                    }

                    createdElement = element.Select(RefreshAndCreateElement).ToList();

                    return true;
                }
                catch (Exception)
                {
                    RefreshElement();
                    return false;
                }
            }, () => "Listbox - GetItemsByName - Unable to get item " + typeof(T) + " within {0})", 30000);

            return createdElement.GroupBy(x => x.Element.Id).Select(x => x.First());
        }

        /// <summary>
        /// Be aware that if a listbox is virtualised, items that are offscreen may not be returned
        /// </summary>
        /// <returns></returns>
        public List<T> GetItems()
        {
            List<T> listItems = null;

            FunctionRunner.RunFuncUntilSuccess(() =>
            {
                try
                {
                    if (IsVirtualisedList) Console.WriteLine("WARNING - ListBox - GetItems - Getting all items for a virtualised list may not return offscreen items!");

                    listItems = GetListItems().Select(RefreshAndCreateElement).ToList();

                    return true;
                }
                catch (Exception)
                {
                    RefreshElement();
                    return false;
                }

            }, () => "Listbox - GetItems - Unable to get items " + typeof(T) + " within {0}", 30000);

            return listItems;
        }

        private T RefreshAndCreateElement(Element element)
        {
            RefreshElement();
            return CreateElement(element);
        }

        protected virtual T CreateElement(Element element)
        {
            return (T) Activator.CreateInstance(typeof(T), element);
        }

        protected bool IsVirtualisedList { get; set; }
    }
}