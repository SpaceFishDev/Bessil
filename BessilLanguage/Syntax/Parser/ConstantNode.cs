namespace BessilLanguage
{
    public sealed class ConstantNode : Node
    {
        public ConstantNode(object value)
        {
            Class = NodeClass.constant;
            Value = value;
        }
        public override IEnumerable<Node> GetChildren()
        {
            return new List<Node>();
        }
    }
}