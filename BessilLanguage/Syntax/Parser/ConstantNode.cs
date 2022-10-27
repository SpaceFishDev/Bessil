namespace BessilLanguage
{
    public sealed class ConstantNode : Node
    {
        public ConstantNode(object value, int line)
        {
            Class = NodeClass.constant;
            Value = value;
            Line = line;
        }
        public override IEnumerable<Node> GetChildren()
        {
            return new List<Node>();
        }
    }
}