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
            while (index < tokens.Count)
            {
                bool parent_is_root = false;
                if (parent != null)
                {
                    parent_is_root = parent.Value == "GLOBAL";
                }
                Token token = tokens[index];
                switch (token.type)
                {
                    case TokenType.TOKEN_EXPR:
                        {
                            if (tokens[index + 1].type == TokenType.TOKEN_PLUS)
                            {
                                BinaryExpressionNode node = new BinaryExpressionNode(
                                    new ConstantNode(token.value), 
                                    new ConstantNode(tokens[index + 2].value), 
                                    true
                                );
                                (root as ScopeNode).AddChild(node);
                                return parse_r(root, node, index + 3);
                            }
                            if (tokens[index + 1].type == TokenType.TOKEN_MINUS)
                            {
                                BinaryExpressionNode node = new BinaryExpressionNode(
                                    new ConstantNode(token.value), 
                                    new ConstantNode(tokens[index + 2].value), 
                                    false
                                );
                                (root as ScopeNode).AddChild(node);
                                return parse_r(root, node, index + 3);
                            }
                            if (tokens[index + 1].type == TokenType.TOKEN_STAR)
                            {

                            }

                            (parent as ScopeNode).AddChild(new ConstantNode(token.value));
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
                                BinaryExpressionNode node = new BinaryExpressionNode((parent as BinaryExpressionNode).right, new ConstantNode(tokens[index + 1].value), false);
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
                                BinaryExpressionNode node = new BinaryExpressionNode( (parent as BinaryExpressionNode).right, new ConstantNode(tokens[index + 1].value), true);
                                (parent as BinaryExpressionNode).right = node;
                                return parse_r(root, node, index + 2); 
                            }
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Cannot add to type: {token.type} LN {token.line}");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Environment.Exit(-1);
                        } break;
                    case TokenType.TOKEN_ENDOFFILE:
                        {
                            return root;
                        }
                    case TokenType.TOKEN_ID:
                        {
                            if (tokens[index + 1].type == TokenType.TOKEN_OPAREN)
                            {
                                CallNode node = new CallNode();
                                ++index;
                                int i = 0;
                                return node;
                            }
                            Environment.Exit(-1);
                        } break;
                    case TokenType.TOKEN_SEMI:
                        {
                            return parse_r(root, root, index + 1);
                        }
                    default:
                        {
                            ++index;
                            return parse_r(root, parent, index + 2);
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
