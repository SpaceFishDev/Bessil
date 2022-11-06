using System.Diagnostics;

namespace BessilLanguage
{
    class Program
    {
        static void Main(string[] Args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string input = PreLexer.includes(File.ReadAllText("main.bsl"));
            Compiler compiler = new Compiler(input, "bc", "out.binary");
            List<byte> bytes = new List<byte>();
            foreach(byte[] b in compiler.CompiledBytes)
            {
                foreach(byte b2 in b)
                {
                    bytes.Add(b2);
                }
            }
            File.WriteAllBytes("out.binary",bytes.ToArray());
            sw.Stop();
            Console.WriteLine($"Compile Time: {sw.Elapsed.TotalMilliseconds} ms");
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
            else if(root.Class == NodeClass.function)
            {
                Console.WriteLine($"NODE( {root.Class}, {(root as FunctionNode).ReturnType}, {root.Value})");
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
        public static void PrettyPrint(Node root)
        {
            PrintR(root, 0);
        }
    }
}