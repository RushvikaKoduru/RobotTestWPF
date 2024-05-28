namespace Test.Common.Controls
{
    public class TextBlock : WindowsElementBase
    {
        public string Text => Element.Text;

        public TextBlock() { }

        public TextBlock(Element element) : base(element)
        {
        }
    }
}
