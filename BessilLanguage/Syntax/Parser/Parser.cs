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
            }
        }
        public Node parse_r(Node root, Node parent, int index)
        {
            while(index < tokens.Count)
            {
                Token token = tokens[index];
                bool parent_is_root = parent.Value == "GLOBAL";
                switch (token.type)
                {
                    case TokenType.TOKEN_EXPR:
                        {
                            if (tokens[index + 1].type == TokenType.TOKEN_PLUS)
                            {
                                BinaryExpressionNode node = new BinaryExpressionNode(new ConstantNode(token.value), new ConstantNode(tokens[index + 2].value), true);
                                return node;
                            }
                            return new ConstantNode(token.value);
                        }
                }
            }
            return null;
        }
        public Node parse()
        {
            return parse_r(new ScopeNode("GLOBAL"), new ScopeNode("GLOBAL"), 0);
        }
    }
}
