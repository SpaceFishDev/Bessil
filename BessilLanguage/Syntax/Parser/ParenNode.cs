using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BessilLanguage
{
    internal class ParenNode : ScopeNode
    {
        public ParenNode( int line) : base("()", line)
        {
            Class = NodeClass.paren;
        }
    }
}
