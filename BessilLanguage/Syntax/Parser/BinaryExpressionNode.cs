namespace BessilLanguage
{
    public class BinaryExpressionNode : Node
    {
        public Node left;
        public Node right;
        int type;
        public enum t{
            ADD,
            SUB,
            MUL,
            DIV,
            ASSIGN,
            BOOLEQ,
            BOOLLESS,
            BOOLMORE,
            VARADD,
            VARSUB,
            VARMUL,
            VARDIV,
        }
        public BinaryExpressionNode(Node left, Node right, t type, int line)
        {
            Line = line;
            string[] data = {"+", "-", "*", "/", "=", "==", "<", ">", "v+", "v-", "v*","v/" };
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
