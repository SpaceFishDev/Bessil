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
        }
        public VariableClass Type;
        public VariableNode(string title, VariableClass @class, Node data)
        {
            this.Value = title;
            this.Type = @class;
            this.Class = NodeClass.var;
            Data = data;
        }

        public override IEnumerable<Node> GetChildren()
        {
            yield return Data;
        }
    }
}
