using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BessilLanguage
{
    internal class TypeChecker
    {
        public static void PutError(string error, int line, bool ln)
        {
            if (ln)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine( "ERROR: " + error + $" LN: {line}");
                Console.ForegroundColor = ConsoleColor.Gray;
                Environment.Exit(-1);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine( "ERROR: " + error);
                Console.ForegroundColor = ConsoleColor.Gray;
                Environment.Exit(-1);
            }
        }
        public static bool IsEquivelantType(VariableNode.VariableClass t, VariableNode.VariableClass a)
        {
            if(t == VariableNode.VariableClass.@int)
            {
                return (a == VariableNode.VariableClass.@int || a == VariableNode.VariableClass.@byte);
            }
            if(t == VariableNode.VariableClass.@long)
            {
                return (a == VariableNode.VariableClass.@int || a == VariableNode.VariableClass.@byte ||  a == VariableNode.VariableClass.@long);
            }
            if(t == VariableNode.VariableClass.@byte)
            {
                return a == VariableNode.VariableClass.@byte;
            }
            return false;
        }
        public static VariableNode.VariableClass GetType(Node n)
        {
            VariableNode.VariableClass type = new VariableNode.VariableClass();
            switch (n.Class)
            {
                case NodeClass.add:
                case NodeClass.sub:
                case NodeClass.mul:
                case NodeClass.div:
                case NodeClass.var_add:
                case NodeClass.var_sub:
                case NodeClass.var_mul:
                case NodeClass.var_div:
                    {
                        return VariableNode.VariableClass.@byte;
                    }
                case NodeClass.var:
                    {
                        return (n as VariableNode).Type;
                    }
                case NodeClass.constant:
                    {
                        if(n.Value == null)
                            return VariableNode.VariableClass.@byte;
                        else if (n.Value.ToString() != null && n.Value.ToString().StartsWith("str:"))
                                return VariableNode.VariableClass.@byte;
                        else if(n.Value.GetType() == Type.GetType("System.Int64"))
                        {
                            if((long)n.Value < char.MaxValue)
                            {
                                return VariableNode.VariableClass.@byte;
                            }
                            if((long)n.Value < int.MaxValue)
                            {
                                return VariableNode.VariableClass.@int;
                            }
                            if((long)n.Value < long.MaxValue)
                            {
                                return VariableNode.VariableClass.@long;
                            }
                        }
                    } break;
            }
            return 0;
        }
        public static void TypeChecking(Node Root)
        {
            TypeCheckTree(Root);
            foreach(var n in Functions)
            {
                if(n.n == "main")
                {
                    return;
                }
            }
            PutError("No entry point defined. Function main must be defined to create executable.", 0, false);
        }
        public static List<(string n , VariableNode.VariableClass type)> Variables = new List<(string, VariableNode.VariableClass)>();
        public static List<(string n, FunctionNode.ReturnTypes type, List<VariableNode.VariableClass> args)> Functions = new List<(string, FunctionNode.ReturnTypes, List<VariableNode.VariableClass>)>();
        private static void TypeCheckTree(Node root)
        {
            if(root == null)
            {
                return;
            }   
            switch (root.Class)
            {
                case NodeClass.function:
                    {
                        FunctionNode func = root as FunctionNode;
                        foreach(var fun in Functions)
                        {
                            if(fun.n == func.Value.ToString())
                            {
                                PutError($"Function {fun} redefined.", func.Line, true);
                            }
                        }
                        List<VariableNode.VariableClass> a = new List<VariableNode.VariableClass>();
                        foreach(Node Child in func.Arguments.GetChildren())
                        {
                            a.Add((Child as VariableNode).Type);
                        }
                        Functions.Add((func.Value.ToString(), func.ReturnType, a));
                        
                    }break;
                case NodeClass.call:
                    {
                        CallNode call = root as CallNode;
                        foreach(var fun in Functions)
                        {
                            if(fun.n == call.Value.ToString())
                            {
                                if(fun.args.Count > call.Arguments.children.Count)
                                {
                                    PutError($"Not enough arguments for function call ({call.Value}).", call.Line, true);
                                }
                                else if (fun.args.Count < call.Arguments.children.Count)
                                {
                                    PutError($"Too many arguments for function call ({call.Value}).", call.Line, true);
                                }
                                for(int i = 0; i < fun.args.Count; ++i)
                                {
                                    if (!IsEquivelantType(fun.args[i],GetType(call.Arguments.children[i])))
                                    {
                                        PutError($"Argument type does not match expected argument in function ({fun.n}). Provided argument is of type <{GetType(call.Arguments.children[i])}> expected is <{fun.args[i]}>", call.Line, true);
                                    }
                                }
                                goto end;
                            }
                        }
                        PutError($"Function {call.Value} is not defined.", call.Line, true);
                    } break;
                case NodeClass.assign:
                    {
                        var type = GetType((root as BinaryExpressionNode).right);
                        string v = (root as BinaryExpressionNode).left.Value.ToString();
                        for(int i = 0; i != Variables.Count; ++i)
                        {
                            if (Variables[i].n == v)
                            {
                                if(!IsEquivelantType(Variables[i].type, type))
                                {
                                    PutError($"Assigning variable of type {Variables[i].type} to type {type}.", root.Line, true);
                                }
                                goto end;
                            }
                        }
                        PutError($"Variable {v} does not exist.", root.Line, true);
                    } break;
                case NodeClass.var:
                    {
                        string title = (root as VariableNode).Value.ToString();
                        foreach(var n in Variables)
                        {
                            if(n.n == title)
                            {
                                PutError($"Variable {title} already exists.", root.Line, true);
                            }
                        }
                        if(!IsEquivelantType((root as VariableNode).Type, GetType((root as VariableNode).Data)))
                        {
                            PutError($"Variable of type {(root as VariableNode).Type} initalized with type {GetType((root as VariableNode).Data)}.", root.Line, true);
                        }
                        Variables.Add((title, (root as VariableNode).Type));
                    } break;
            }
            end:
            foreach(var Child in root.GetChildren())
            {
                TypeCheckTree(Child);
            }
        }
    }
}
