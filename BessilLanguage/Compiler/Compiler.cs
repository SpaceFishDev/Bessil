using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BessilLanguage
{
    internal class Compiler
    {
        public Parser parser;
        public Node root;
        public string Assembly = "";
        public Compiler(string source, string output_option)
        {
            parser = new Parser(source);
            root = parser.parse();
            Program.PrettyPrint(root);
            TypeChecker.TypeChecking(root);
            if(output_option == "x86")
                CompileX86(root);
        }
        public void assembly(string asm)
        {
            Assembly += asm + "\n";
        }
        public int GetSizeOfVar(VariableNode.VariableClass type)
        {
            switch (type)
            {
                case VariableNode.VariableClass.@byte:
                    {
                        return 1;
                    }
                case VariableNode.VariableClass.@int:
                    {
                        return 8;
                    }
                case VariableNode.VariableClass.@long:
                    {
                        return 16;
                    }
            }
            return 4;

        }
        List<(string variable, int offset, bool isStatic)> Variables = new List<(string variable, int offset, bool isStatic)>();
        int StringIndex = 0;
        public void CompileX86(Node root)
        {
            if(root == null)
            {
                return;
            }
            switch (root.Class)
            {
                case NodeClass.function:
                    {
                        assembly($"{(root as FunctionNode).Value}:");
                        assembly("push rbp");
                        assembly("mov rbp, rsp");
                        assembly("mov rbx, rbp"); // pointer to first argument just add to this to get the args
                        foreach (var Arg in (root as FunctionNode).Arguments.children)
                        {
                            Variables.Add(
                                (
                                    Arg.Value.ToString(),
                                    GetSizeOfVar((Arg as VariableNode).Type),
                                    false
                                )
                            );
                        }
                    }break;
                case NodeClass.call:
                    {
                        foreach(var Arg in (root as CallNode).Arguments.children)
                        {
                            if (TypeChecker.GetType(Arg) == VariableNode.VariableClass.@byte && Arg.Class != NodeClass.var)
                            {
                                if(Arg.Value.ToString().StartsWith("str:"))
                                {
                                    string data = Arg.Value.ToString().Replace("str:", "");
                                    Assembly = $"ST{StringIndex}:\ndb {data},0\n" + Assembly;
                                    assembly($"mov rax, rbx");
                                    assembly($"mov rbx, byte ST{StringIndex}");
                                    assembly($"push rbx");
                                    assembly($"push rax");
                                    assembly($"mov rbx, rax");
                                    assembly($"pop rax");
                                    ++StringIndex;
                                }
                                else
                                {
                                    assembly($"push byte {Arg.Value}");
                                }
                            }
                            assembly($"call {root.Value}");
                        }
                    } break;
                case NodeClass.assembly:
                    {
                        assembly(root.Value.ToString());
                    } break;
            }
            foreach (var child in root.GetChildren())
            {
                CompileX86(child);
            }
            if(root.Class == NodeClass.function)
            {
                assembly("mov rsp, rbp");
                assembly("pop rbp");
                assembly("ret");
            }
        }
        
    }
}
