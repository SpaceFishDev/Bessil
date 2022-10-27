using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BessilLanguage
{
    public class FunctionNode : ScopeNode
    {
        public bool ArgDefine = true;
        public bool ReturnDefine = false;

        public ScopeNode Arguments;

        public Node ReturnValue;
        public enum ReturnTypes
        {
            @byte,
            @int,
        }
        public ReturnTypes ReturnType;
        public FunctionNode(string title, ReturnTypes returnType, int line) : base(title, line)
        {
            Class = NodeClass.function;
            ReturnType = returnType;
            Arguments = new ScopeNode(title + ":Arguments", line);
        }
        public override IEnumerable<Node> GetChildren()
        {
            yield return Arguments;
            foreach(Node nd in children)
            {
                yield return nd;
            }
            yield return ReturnValue;
        }
    }
}
