namespace BessilLanguage
{
    public sealed class BinaryExpressionNode : Node
    {
        public Node left;
        public Node right;
        bool add;
        public BinaryExpressionNode(Node left, Node right, bool plus)
        {
            this.left = left;
            this.right = right;
            add = plus;
            Class = (add) ? NodeClass.add : NodeClass.sub;
            Value = (add) ? "+" : "-";
        }
        public override IEnumerable<Node> GetChildren()
        {
            yield return left;
            yield return right;
        }
    }
}
