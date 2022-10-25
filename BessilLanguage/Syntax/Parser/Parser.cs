#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BessilLanguage
{
    public class Parser
    {
        public List<Token> tokens;
        public string source;
        public Lexer lexer;
        public Parser(string source)
        {
            this.source = source;
            tokens = new List<Token>();
            lexer = new Lexer(source);
            while (true)
            {
                Token token = lexer.lex();
                tokens.Add(token);
                if(token.type == TokenType.TOKEN_ENDOFFILE)
                {
                    tokens.Add(token);
                    return;
                }
            }

        }
        public Node parse_r(Node root, Node parent, int index)
        {
            if (index < tokens.Count)
            {
                bool parent_is_root = false;
                if (parent != null)
                {
                    parent_is_root = parent.Value == "GLOBAL";
                }
                Token token = tokens[index];
                if (root.Class == NodeClass.function && 
                        (
                                token.type == TokenType.TOKEN_EXPR 
                            || token.type == TokenType.TOKEN_PLUS 
                            || token.type == TokenType.TOKEN_MINUS 
                            || token.type == TokenType.TOKEN_SLASH 
                            || token.type == TokenType.TOKEN_STAR
                        )
                    )
                {
                    if ((root as FunctionNode).ArgDefine)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Cannot put expression inside of the arguments of a function. LN {token.line}");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Environment.Exit(-1);
                    }
                }
                switch (token.type)
                {
                    case TokenType.TOKEN_ENDOFFILE:
                        {
                            return root;
                        }
                    case TokenType.TOKEN_EXPR:
                        {
                            if (parent.Class != NodeClass.var)
                            {
                                switch (tokens[index + 1].type)
                                {

                                    case TokenType.TOKEN_PLUS:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(token.value),
                                                new ConstantNode(tokens[index + 2].value),
                                                BinaryExpressionNode.t.ADD
                                             );
                                            (root as ScopeNode).AddChild(node);
                                            return parse_r(root, node, index + 3);
                                        }
                                    case TokenType.TOKEN_MINUS:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(token.value),
                                                new ConstantNode(tokens[index + 2].value),
                                                BinaryExpressionNode.t.SUB
                                            );
                                            (root as ScopeNode).AddChild(node);
                                            return parse_r(root, node, index + 3);
                                        }
                                    case TokenType.TOKEN_STAR:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                            new ConstantNode(token.value),
                                            new ConstantNode(tokens[index + 2].value),
                                            BinaryExpressionNode.t.MUL
                                           );
                                            (root as ScopeNode).AddChild(node);
                                            return parse_r(root, node, index + 3);
                                        }
                                    case TokenType.TOKEN_SLASH:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                            new ConstantNode(token.value),
                                            new ConstantNode(tokens[index + 2].value),
                                            BinaryExpressionNode.t.DIV
                                           );
                                            (root as ScopeNode).AddChild(node);
                                            return parse_r(root, node, index + 3);
                                        }
                                    case TokenType.TOKEN_SEMI:
                                        {
                                            ConstantNode node = new ConstantNode(token.value);
                                            (root as ScopeNode).AddChild(node);
                                            return parse_r(root, root, index + 1);
                                        }
                                    default:
                                        {
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            Console.WriteLine($"Unexpected token {tokens[index + 1].type} LN {tokens[index + 1].line}");
                                            Console.ForegroundColor = ConsoleColor.Gray;
                                            Environment.Exit(-1);
                                            return null;
                                        }
                                }
                            }
                            else
                            {
                                switch (tokens[index + 1].type)
                                {

                                    case TokenType.TOKEN_PLUS:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(token.value),
                                                new ConstantNode(tokens[index + 2].value),
                                                BinaryExpressionNode.t.ADD
                                             );
                                            (parent as VariableNode).Data = node;
                                            return parse_r(root, node, index + 3);
                                        }
                                    case TokenType.TOKEN_MINUS:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(token.value),
                                                new ConstantNode(tokens[index + 2].value),
                                                BinaryExpressionNode.t.SUB
                                            );
                                            (parent as VariableNode).Data = node;
                                            return parse_r(root, node, index + 3);
                                        }
                                    case TokenType.TOKEN_STAR:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                            new ConstantNode(token.value),
                                            new ConstantNode(tokens[index + 2].value),
                                            BinaryExpressionNode.t.MUL
                                           );
                                            (parent as VariableNode).Data = node;
                                            return parse_r(root, node, index + 3);
                                        }
                                    case TokenType.TOKEN_SLASH:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                            new ConstantNode(token.value),
                                            new ConstantNode(tokens[index + 2].value),
                                            BinaryExpressionNode.t.DIV
                                           );
                                            (parent as VariableNode).Data = node;
                                            return parse_r(root, node, index + 3);
                                        }
                                    case TokenType.TOKEN_SEMI:
                                        {
                                            (parent as VariableNode).Data = new ConstantNode(tokens[index].value);
                                            return parse_r(root, root, index + 1);
                                        }
                                }
                            }
                            (root as ScopeNode).AddChild(new ConstantNode(token.value));
                            return parse_r(root, parent, index + 2);
                        }
                    case TokenType.TOKEN_MINUS:
                        {
                            if(index == 0)
                            {
                                throw new Exception("Unexpected number of tokens.");
                            }
                            if (tokens[index + 1].type == TokenType.TOKEN_EXPR && tokens[index -1].type == TokenType.TOKEN_EXPR)
                            {
                                BinaryExpressionNode node = new BinaryExpressionNode(
                                    (parent as BinaryExpressionNode).right, 
                                    new ConstantNode(tokens[index + 1].value), 
                                    BinaryExpressionNode.t.SUB
                                );
                                (parent as BinaryExpressionNode).right = node;
                                return parse_r(root, node, index + 2);
                            }
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Cannot add to type: {token.type} LN {token.line}");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Environment.Exit(-1);
                        } break;
                    case TokenType.TOKEN_PLUS:
                        {
                            if (tokens[index + 1].type == TokenType.TOKEN_EXPR)
                            {   
                                BinaryExpressionNode node = new BinaryExpressionNode( 
                                    (parent as BinaryExpressionNode).right,
                                    new ConstantNode(tokens[index + 1].value),
                                    BinaryExpressionNode.t.ADD
                                );
                                (parent as BinaryExpressionNode).right = node;
                                return parse_r(root, node, index + 2); 
                            }
                            Console.ForegroundColor = ConsoleColor.Red; 
                            Console.WriteLine($"Cannot add to type: {token.type} LN {token.line}");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Environment.Exit(-1);
                        } break;
                    case TokenType.TOKEN_STAR:
                        {
                            if (tokens[index + 1].type == TokenType.TOKEN_EXPR)
                            {
                                BinaryExpressionNode node = new BinaryExpressionNode(
                                    (parent as BinaryExpressionNode).right,
                                    new ConstantNode(tokens[index + 1].value), 
                                    BinaryExpressionNode.t.MUL
                                );
                                (parent as BinaryExpressionNode).right = node;
                                return parse_r(root, 
                                    node,
                                    index + 2
                                );
                            }
                           
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Cannot multiply type: {token.type} LN {token.line}");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Environment.Exit(-1);
                        } break;
                    case TokenType.TOKEN_SLASH:
                        {
                            if (tokens[index + 1].type == TokenType.TOKEN_EXPR)
                            {
                                BinaryExpressionNode node = new BinaryExpressionNode(
                                    (parent as BinaryExpressionNode).right, 
                                    new ConstantNode(tokens[index + 1].value), 
                                    BinaryExpressionNode.t.DIV
                                );
                                (parent as BinaryExpressionNode).right = node;
                                return parse_r(root, 
                                    node, 
                                    index + 2
                                );
                            }
                           
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Cannot multiply type: {token.type} LN {token.line}");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Environment.Exit(-1);
                        } break;
                    case TokenType.TOKEN_SEMI:
                        {
                            if (root.Value.ToString().Contains(":Return"))
                            {
                                return parse_r(
                                    (root as ScopeNode).Root,
                                    (root as ScopeNode).Root, 
                                    index + 1
                                );
                            }
                            return parse_r(
                                root, 
                                root,
                                index + 1
                            );
                        }
                    case TokenType.TOKEN_FUNC:
                        {
                            if (tokens[index + 1].type != TokenType.TOKEN_ID)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Function declaration requires types: FUNC ID FUNCTYPE ... BEGIN.  LN {token.line}");
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Environment.Exit(-1);
                                return null;
                            }
                            switch (tokens[index + 2].type)
                            {
                                case TokenType.TOKEN_BYTE:
                                case TokenType.TOKEN_INT:
                                    {
                                    } break;
                                default:
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine($"Function declaration requires types: FUNC ID FUNCTYPE ... BEGIN.  LN {token.line}");
                                        Console.ForegroundColor = ConsoleColor.Gray;
                                        Environment.Exit(-1);
                                        return null;
                                    }
                            }
                            FunctionNode.ReturnTypes ret = FunctionNode.ReturnTypes.@byte;
                            switch(tokens[index + 2].type)
                            {
                                case TokenType.TOKEN_INT:
                                    {
                                        ret = FunctionNode.ReturnTypes.@int;
                                    }
                                    break;
                            }
                            FunctionNode node = new FunctionNode(tokens[index + 1].value.ToString(), ret);
                            node.Root = root;
                            return parse_r(
                                node, 
                                node, 
                                index + 3
                            );
                        }
                    case TokenType.TOKEN_BEGIN:
                        {
                            if(parent.Class != NodeClass.function)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Function declaration requires types: FUNC ID ... BEGIN.  LN {token.line}");
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Environment.Exit(-1);
                                return null;
                            }
                            (root as FunctionNode).ArgDefine = false;
                            return parse_r(
                                (root as ScopeNode), 
                                parent, 
                                index + 1
                            );
                        }
                    case TokenType.TOKEN_END:
                        {
                            if (root.Value != "GLOBAL" && root.Class != NodeClass.scope)
                            {
                                ((root as ScopeNode).Root as ScopeNode).AddChild(root);
                                return parse_r(
                                    (root as ScopeNode).Root, 
                                    (root as ScopeNode).Root, 
                                    index + 1
                                );
                            }
                            return parse_r(root, 
                                root, 
                                index + 1
                            );
                        }
                    case TokenType.TOKEN_BYTE:
                        {
                            if (root.Class == NodeClass.function && (root as FunctionNode).ArgDefine == true)
                            {
                                if (index + 1 > tokens.Count)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($"Argument declaration enough arguments REQUIRED : VARTYPE NAME ;. LN {token.line}");
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                    Environment.Exit(-1);
                                }
                                if (tokens[index + 1].type != TokenType.TOKEN_ID)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($"Argument declaration without a name. LN {token.line}");
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                    Environment.Exit(-1);
                                }
                                VariableNode node = new VariableNode(
                                    tokens[index + 1].value.ToString(), 
                                    VariableNode.VariableClass.@byte, 
                                    new ConstantNode(null)
                                );
                                ((root as FunctionNode).Arguments as ScopeNode).AddChild(node);
                                return parse_r(root, 
                                    parent, 
                                    index + 2
                                );
                            }
                            if (root.Class == NodeClass.scope || root.Class == NodeClass.function)
                            {
                                if(index + 3 > tokens.Count)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($"Variable declaration enough arguments REQUIRED : VARTYPE NAME = DECLARATION OR NULL;. LN {token.line}");
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                    Environment.Exit(-1);
                                }
                                if(tokens[index + 1].type != TokenType.TOKEN_ID)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($"Variable declaration without a name. LN {token.line}");
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                    Environment.Exit(-1);
                                }
                                if (tokens[index + 2].type != TokenType.TOKEN_EQ)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($"Variable declaration without a '=' sign. LN {token.line}");
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                    Environment.Exit(-1);
                                }
                                if (tokens[index + 3].type == TokenType.TOKEN_NULL)
                                {
                                    VariableNode n = new VariableNode(
                                        tokens[index + 1].value.ToString(), 
                                        VariableNode.VariableClass.@byte, 
                                        new ConstantNode(null)
                                    );
                                    (root as ScopeNode).AddChild(n);
                                    return parse_r(
                                        root, 
                                        parent, 
                                        index + 4
                                    );
                                }
                                if (tokens[index + 3].type == TokenType.TOKEN_STRING)
                                {
                                    VariableNode n = new VariableNode(
                                        tokens[index + 1].value.ToString(),
                                        VariableNode.VariableClass.@byte,
                                        new ConstantNode(tokens[index + 3].value)
                                    );
                                    (root as ScopeNode).AddChild(n);
                                    return parse_r(root, n, index + 4);
                                }
                                VariableNode node = new VariableNode(
                                    tokens[index + 1].value.ToString(), 
                                    VariableNode.VariableClass.@byte, 
                                    null
                                );
                                (root as ScopeNode).AddChild(node);
                                return parse_r(
                                    root, 
                                    node, 
                                    index + 3
                                );
                            }
                            Console.WriteLine("UNREACHABLE, so explain to me, what? WHAT? how buddy, how did you manage to get the variable to be declared outside of global??? and outside of a function so its floating in space!? How did you? what?!?");
                            return null;
                        }
                    case TokenType.TOKEN_RETURN:
                        {
                            VariableNode node = new VariableNode(
                                root.Value + ":Return", 
                                (VariableNode.VariableClass)(root as FunctionNode).ReturnType, 
                                null
                            );
                            if(root.Class != NodeClass.function)
                            {
                                Console.WriteLine($"Returning outside of a function scope LN {token.line}");
                                Environment.Exit(-1);
                            }
                            (root as FunctionNode).ReturnValue = node;
                            (root as FunctionNode).ReturnDefine = true;
                            return parse_r(
                                root, 
                                parent, 
                                index + 1
                            );
                        }
                    default:
                        {
                            ++index;
                            return parse_r(
                                root, 
                                parent, 
                                index + 1
                            );
                        }
                }
            }
            return root;
        }
        public Node parse()
        {
            return parse_r(new ScopeNode("GLOBAL"), new ScopeNode("GLOBAL"), 0);
        }
    }
}
