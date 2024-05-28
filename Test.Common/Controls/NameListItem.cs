namespace Test.Common.Controls
{
    /// <summary>
    /// Generic List Item element which has a Name property
    /// </summary>
    public class NameListItem : WindowsElementBase
    {
        public NameListItem(Element element) : base(element) { }

        public TextBlock Name => new TextBlock(this.TryGetElement("ListItemName"));
    }
}
