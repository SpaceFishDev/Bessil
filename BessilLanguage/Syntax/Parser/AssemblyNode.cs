using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BessilLanguage
{
    internal class AssemblyNode : Node
    {
        public AssemblyNode(object Value, int Line)
        {
            this.Value = Value;
            this.Line = Line;
            Class = NodeClass.assembly;
        }
        public override IEnumerable<Node> GetChildren()
        {
            return new List<Node>();
        }
    }
}
