using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BessilLanguage
{
    internal class IfNode : FunctionNode
    {
        public IfNode(int line) : base("if", 0, line)
        {
            Class = NodeClass.if_node;
        }
    }
}
