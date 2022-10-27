namespace BessilLanguage
{
    public class ScopeNode : Node
    {
        public List<Node> children;
        public Node Root;
        public ScopeNode(string title, int line)
        {
            this.Class = NodeClass.scope;
            this.Value = title;
            this.children = new List<Node>();
            Line = line;
        }
        public void AddChild(Node child)
        {
            children.Add(child);
        }
        public override IEnumerable<Node> GetChildren()
        {
            return children;
        }
    }

}
