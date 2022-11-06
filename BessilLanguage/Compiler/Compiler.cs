using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
/*
 PROBLEM:
move it into bp, instead of pushing it.
 */

namespace BessilLanguage
{
    internal class Compiler
    {
        /*
         * First 2 bytes are the number of arguments.(Not instructions you big dope)
         * Last 2 bytes are the opcode
         */
        public enum Opcodes
        {
            ADD = 0x00020015, // ADD [address] [address]
            MUL = 0x00020016, // MUL [address] [address]
            SUB = 0x00020017, // SUB [address] [address]
            DIV = 0x00020018, // DIV [address] [address]
            PUSH = 0x00010000, // PUSH [DATA]
            PUSHMEM = 0x00010001, // PUSH [DATA]
            POP = 0x00010002, // POP [ADRESS]
            READPTR = 0x00010012 , // READPTR [ADRESS TO READ] (PUSHES RESULT TO STACK)
            READPTRMEM = 0x00020013, // READPTRMEM [ADRESS (WHERE TO READ)] [ADRESS (WHERE TO PUT IT)]
            SET = 0x00020003, // SET [ADRESS] [DATA]
            MOV = 0x00020004, // MOV [ADRESS] [ADRESS]
            CMP = 0x00020005, // CMP [ADRESS] [ADRESS] (pushes result to the stack if equal pushes 0 if less pushes -1 if more pushes 1 if not equal it pushes 2)
            JMP = 0x00010006, // JMP [ADRESS] (jumps to the given adress)
            JZ = 0x00010007, // JE [ADRESS] (jumps if the top value in the stack is zero   )
            JN = 0x00010008, // JN [ADRESS] (jumps if top value in the stack is negative)
            JNE = 0x00010013, // JNE [ADRESS] (jump if not equal, so if the top of the stack equals 2)
            JP = 0x00010009, // JP [ADRESS] (jumps if top value in the stack is positive and not 0)
            JMP_STACK = 0x0000012, // JUMPS TO WHAT IS STORED AT THE TOP OF THE STACK
            BUILTIN = 0x00200010, // BUILTIN CALL LIKE PRINT OR READ CHAR (only works in interpreted mode)  {BUILTIN [CALL] [NUM ARG] [...ARGS] [RETURN ADRESS]}
            INLINE = 0x00020011, // INLINE ASSEMBLY GOES HERE (actual assembly. (OR MASLOS)) {INLINE [length] [all characters]} (Only works in compiled mode)
            EXIT = 0x00000014, // Forces exit
        }
        
        [DllImport("msvcrt.dll")]
        public static extern int system(string format);
        public Parser parser;
        public Node root;
        public List<byte[]> CompiledBytes = new List<byte[]>();
        public Compiler(string source, string output_option, string output_file)
        {
            parser = new Parser(source);
            root = parser.parse();
            if (output_option == "bc")
            {
                CompileBC(root);
                List<long[]> bc_new = new List<long[]>();
                int ad = 0;
                foreach (var f in Functions)
                {
                    if (f.func == "main")
                    {
                        ad = f.ic;
                    }
                }
                bc_new.Add(new long[] { (long)Opcodes.JMP, ad });
                if (output_file != "")
                {
                    Program.PrettyPrint(root);
                    TypeChecker.TypeChecking(root);
                    foreach (var BC in bc)
                    {
                        bc_new.Add(BC);
                    }
                    bc = bc_new;
                    foreach (var BC in bc)
                    {
                        foreach (long b in BC)
                        {
                            if (b == BC[0])
                            {
                                Console.WriteLine();
                                Console.Write((Opcodes)BC[0] + " ");
                            }
                            else
                            {
                                Console.Write(b + " ");
                            }
                            CompiledBytes.Add(BitConverter.GetBytes(b));
                        }
                    }
                    Console.WriteLine();
                }
            }

        }
        List<long[]> bc = new List<long[]>();
        int IC = 1;
        long mem = 0;
        string current_func = "glbl";
        List<(string func, int ic)> Functions = new List<(string func, int ic)>();
        List<(string var, long mem, string current)> Variables = new List<(string var, long mem, string current)>();
        long RES = 1024 * 16;
        long ic_reg = 1024 * 8;
        public void CompileBC(Node root)
        {
            if(root == null) 
            {
                return;
            }
            if (root.Value.ToString().EndsWith(":Arguments"))
                return;
            switch (root.Class)
            {
                case NodeClass.function: 
                {
                        Functions.Add((root.Value.ToString(), IC - 1 ));
                        current_func = root.Value.ToString();
                        foreach(var arg in (root as FunctionNode).Arguments.children)
                        {
                            bc.Add(new long[] { (long)Opcodes.SET, RES, 0});
                            ++RES;
                        }
                } break;
                case NodeClass.if_node:
                {
                    current_func = root.Value.ToString();
                    foreach(var argument in (root as IfNode).Arguments.children)
                    {
                            switch (argument.Class)
                            {
                                case NodeClass.bool_eq:
                                    {
                                        if ((argument as BinaryExpressionNode).left.Class == NodeClass.constant && (argument as BinaryExpressionNode).right.Class == NodeClass.constant)
                                        {
                                            if (char.IsDigit(((argument as BinaryExpressionNode).right as ConstantNode).Value.ToString()[0]))
                                            {
                                                int EndIc = 0;
                                                Compiler comp_temp = new Compiler("", "bc", "");
                                                foreach (var child in root.GetChildren())
                                                {
                                                    comp_temp.CompileBC(child);
                                                }
                                                EndIc = comp_temp.IC;
                                                //if (!f)
                                                    //TypeChecker.PutError($"Variable {(argument as BinaryExpressionNode).left.Value} does not exist in current context.", argument.Line, true);
                                                bc.Add(new long[] { (long)Opcodes.SET, RES++, (long)(argument as BinaryExpressionNode).right.Value});
                                                long addr = 0;
                                                bool f = false;
                                                foreach(var Var in Variables)
                                                {
                                                    if(Var.current == current_func && Var.var == (argument as BinaryExpressionNode).left.Value.ToString()) 
                                                    {
                                                        f = true;
                                                        addr = Var.mem;    
                                                    }
                                                }
                                                bc.Add(new long[] { (long)Opcodes.CMP, RES - 1,addr});
                                                bc.Add(new long[] {(long)Opcodes.JZ, EndIc + 3});
                                            }
                                        } 
                                    } break;
                            }
                    }
                } break;
                case NodeClass.call:
                {
                        bc.Add(new long[] { (long)Opcodes.SET, ic_reg, IC});
                        foreach(var Func in Functions)
                        {
                            if(root.Value.ToString() == Func.func)
                            {
                                foreach(var Arg in (root as CallNode).Arguments.children)
                                {
                                    if(Arg.Class == NodeClass.constant)
                                    {
                                        if (Arg.Value.ToString().StartsWith("str:"))
                                        {
                                            string st = Arg.Value.ToString().Replace("str:", "");
                                            foreach (char c in st)
                                                bc.Add(new long[] { (long)Opcodes.PUSH, c });
                                            bc.Add(new long[] { (long)Opcodes.PUSH, 0 });
                                            mem += st.Length + 1;
                                        }
                                        else if (char.IsDigit(Arg.Value.ToString()[0]))
                                        {
                                            bc.Add(new long[] {(long)Opcodes.PUSH, (long)Arg.Value});
                                        }
                                        else
                                        {
                                            foreach(var Variable in Variables)
                                            {
                                                if(Variable.var == Arg.Value.ToString() && Variable.current == current_func)
                                                {
                                                    bc.Add(new long[] {(long) Opcodes.PUSHMEM, Variable.mem});
                                                }else if(Variable.var == Arg.Value.ToString() && Variable.current != current_func)
                                                {
                                                    int i = 0;
                                                    List<(string f, int x)> vars = new List<(string f, int x)>();
                                                    foreach(var Var in Variables)
                                                    {
                                                        if(Var.var == Arg.Value.ToString())
                                                        {
                                                            vars.Add((Var.current, i));
                                                        }
                                                        ++i;
                                                    }
                                                    foreach(var var in vars)
                                                    {
                                                        if(var.f == current_func)
                                                        {
                                                            bc.Add(new long[] { (long)Opcodes.PUSHMEM, Variables[var.x].mem});
                                                            goto end;
                                                        }
                                                    }
                                                    Console.WriteLine(Variable.current);
                                                    TypeChecker.PutError($"Variable {Variable.var} doesn't exist in current context. ", root.Line, true);
                                                    end:;
                                                } 
                                            }
                                        }
                                    }
                                }
                                bc.Add(new long[] {(long)Opcodes.JMP, Func.ic});
                            }
                        }
                }break;
                case NodeClass.var:
                {
                    if((root as VariableNode).Data.Class == NodeClass.constant)
                    {
                            VariableNode node = root as VariableNode;
                            if (node.Data.Value == null) node.Data.Value = 0;
                            int i = 0;
                            if (node.Data.Value.ToString().StartsWith("str:"))
                            {
                                foreach(char c in node.Data.Value.ToString().Replace("str:",""))
                                {
                                    bc.Add(new long[] { (long)Opcodes.SET,mem, (long)c});
                                    ++mem;
                                    ++i;
                                }
                                bc.Add(new long[] { (long)Opcodes.SET, mem, (long)0});
                                ++mem;
                                ++i;
                            }
                            else if (char.IsDigit((root as VariableNode).Data.Value.ToString()[0]))
                            {
                                bc.Add(new long[] { (long)Opcodes.SET, mem, (long)node.Data.Value });
                                ++mem;
                            }
                            Variables.Add((node.Value.ToString(), char.IsDigit((root as VariableNode).Data.Value.ToString()[0]) ? mem - 1 : mem - i, current_func));
                        }
                        
                } break;
                case NodeClass.assembly:
                {
                    string[] args = root.Value.ToString().Split(" ");
                    List<long> arguments = new List<long>();
                    foreach(string arg in args)
                    {
                            if (string.IsNullOrEmpty(arg))
                            {
                                continue;
                            }
                            try
                            {
                                long x = long.Parse(arg);
                                arguments.Add(x);
                            }
                            catch
                            {
                                TypeChecker.PutError("ALL ASSEMBLY ARGUMENTS IN INTERPRETED MODE (OR BYTE CODE) MUST BE ENUMERABLE VALUES", root.Line, true);
                            }
                    }
                    bc.Add(arguments.ToArray());
                } break;
            }
            ++IC;
            foreach (Node child in root.GetChildren())
            {
                CompileBC(child);
            }
            if(root.Class == NodeClass.function && root.Value.ToString() != "main")
             {
                bc.Add(new long[] {(long)Opcodes.READPTR, ic_reg});
                bc.Add(new long[] { (long)Opcodes.JMP_STACK} );
            }else if(root.Class == NodeClass.function)
            {
                bc.Add(new long[] { (long)Opcodes.EXIT});
            }
        }
    }
}