namespace BessilLanguage
{
    class Program
    {
        static void Main(string[] Args)
        {
            while (true)
            {
                string input = Console.ReadLine();
                if (input[0] == '$')
                {
                    switch (input)
                    {
                        case "$cls":
                        case "$clear":
                            {
                                Console.Clear();
                                continue;
                            }
                        case "$exit":
                            {
                                return;
                            }
                    }
                }
                Parser parser = new Parser(input);
                Node root = parser.parse();
                PrettyPrint(root);
            }
        }

        static void PrintR(Node root, int level)
        {
            for (int i = 0; i < level; ++i, Console.Write("\t")) ;
            switch (root.Class) 
            {
                case NodeClass.scope:
                    {
                        Console.WriteLine(root.Value);
                        ++level;
                        foreach(Node Child in (root as ScopeNode).GetChildren())
                        {
                            PrintR(Child, level);
                        }
                    } break;
                case NodeClass.add:
                case NodeClass.sub:
                    {
                        Console.WriteLine(root.Value);
                        PrintR((root as BinaryExpressionNode).left, ++level);
                        PrintR((root as BinaryExpressionNode).right, level);
                    } break;
                default:
                    {
                        Console.WriteLine(root.Value);
                        foreach (Node Child in root.GetChildren())
                        {
                            PrintR(Child, level);
                        }
                    } break;
            }
        }
        static void PrettyPrint(Node root)
        {
            PrintR(root, 0);
        }
    }
}