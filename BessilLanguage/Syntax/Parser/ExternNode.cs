using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BessilLanguage
{
    internal class ExternNode : Node
    {
        public ExternNode(object Value, int Line)
        {
            this.Value = Value;
            this.Line = Line;
            Class = NodeClass.@extern;
        }
        public override IEnumerable<Node> GetChildren()
        {
            return new List<Node>();
        }
    }
}
