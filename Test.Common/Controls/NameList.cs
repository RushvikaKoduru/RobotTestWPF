using System.Collections.Generic;
using System.Linq;


namespace Test.Common.Controls
{
    /// <summary>
    /// Generic List box containing items which have a name text block property
    /// </summary>
    public class NameList : ListBox<NameListItem>
    {

        public NameList(Element element) : base(element, "ListBoxItem") { }

        public IList<string> GetNames => GetItems().Select(x => x.Name.Text).ToList();

        protected override NameListItem CreateElement(Element element)
        {
            return new NameListItem(element);
        }
    }
}
