using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Common
{
    public abstract class ViewBase : WindowsElementBase
    {
        protected ViewBase(Element element) : base(element)
        {
        }
    }
}
