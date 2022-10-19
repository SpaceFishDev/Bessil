namespace BessilLanguage
{
    public sealed class ConstantNode : Node
    {
        public ConstantNode(object value)
        {
            Class = NodeClass.constant;
            Value = value;
        }
    }
}