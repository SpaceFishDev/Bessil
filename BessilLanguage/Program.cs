﻿namespace BessilLanguage
{
    class Program
    {
        static void Main(string[] Args)
        {
            string input = PreLexer.includes(File.ReadAllText("main.bsl"), new List<string>());
            Parser parser = new Parser(input);
            Node root = parser.parse();
            PrettyPrint(root);
        }

        static void PrintR(Node root, int level)
        {
            if (root == null)
                return;
            for (int i = 0; i < level; ++i, Console.Write("\t")) ;
            if (root.Class == NodeClass.var)
            {   
                Console.WriteLine($"NODE( {root.Class}, {(root as VariableNode).Type}, {root.Value})");
            }
            else
            {
                Console.WriteLine($"NODE( {root.Class}, {root.Value} )");
            }
            ++level;
            foreach (Node Child in root.GetChildren())
            {   
                PrintR(Child, level);
            }
        }
        static void PrettyPrint(Node root)
        {
            PrintR(root, 0);
        }
    }
}