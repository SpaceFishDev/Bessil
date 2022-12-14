using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BessilLanguage
{
    internal class VariableNode : Node
    {
        public Node Data;
        public enum VariableClass
        {
            @byte,
            @int,
            @long,
        }
        public VariableClass Type;
        public VariableNode(string title, VariableClass @class, Node data, int line)
        {
            this.Value = title;
            this.Type = @class;
            this.Class = NodeClass.var;
            Data = data;
            Line = line;
        }

        public override IEnumerable<Node> GetChildren()
        {
            yield return Data;
        }
    }
}
