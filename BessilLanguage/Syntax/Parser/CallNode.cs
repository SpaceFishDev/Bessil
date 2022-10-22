using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BessilLanguage
{
    public sealed class CallNode : Node
    {
        public List<Node> arguments;
        public CallNode()
        {
            arguments = new List<Node>();
            Class = NodeClass.call;
        }

        public override IEnumerable<Node> GetChildren()
        {
            return arguments;
        }
    }

}
