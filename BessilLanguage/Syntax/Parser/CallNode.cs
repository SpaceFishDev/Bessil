using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BessilLanguage
{
    internal class CallNode : Node
    {
        public ScopeNode Arguments;

        public CallNode(string function, int line)
        {
            Value = function;
            Class = NodeClass.call;
            Arguments = new ScopeNode(function + ":Arguments", line);
            Line = line;
            Arguments.Root = this;
        }

        public override IEnumerable<Node> GetChildren()
        {
            return Arguments.children;
        }
    }
}
