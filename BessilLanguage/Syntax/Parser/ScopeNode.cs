namespace BessilLanguage
{
    public sealed class ScopeNode : Node
    {
        public List<Node> children;
        public ScopeNode(string title)
        {
            this.Class = NodeClass.scope;
            this.Value = title;
            this.children = new List<Node>();
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
