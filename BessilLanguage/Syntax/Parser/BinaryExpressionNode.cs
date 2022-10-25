namespace BessilLanguage
{
    public sealed class BinaryExpressionNode : Node
    {
        public Node left;
        public Node right;
        int type;
        public enum t{
            ADD,
            SUB,
            MUL,
            DIV,
        }
        public BinaryExpressionNode(Node left, Node right, t type)
        {
            string data = "+-*/";
            this.left = left;
            this.right = right;
            this.type =(int)type;
            Class = (NodeClass)type;
            Value = data[(int)Class].ToString();
        }
        public override IEnumerable<Node> GetChildren()
        {
            yield return left;
            yield return right;
        }
    }
}
