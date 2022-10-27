#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast

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
                if (token.type == TokenType.TOKEN_ENDOFFILE)

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
                        TypeChecker.PutError("Expression cannot be put into the argument list of a function definition.", token.line, true);
                    }
                }
                switch (token.type)
                {
                    case TokenType.TOKEN_ENDOFFILE:
                        {
                            return root;
                        }
                    case TokenType.TOKEN_STRING:
                        {
                            if (parent.Class == NodeClass.call)
                            {
                                (parent as CallNode).Arguments.AddChild(new ConstantNode(token.value, token.line));
                                return parse_r(
                                    root,
                                    parent,
                                    index + 1
                                );
                            }
                        }
                        break;
                    case TokenType.TOKEN_COMMA:
                        {
                            if (parent.Class == NodeClass.call)
                            {
                                return parse_r(
                                    root,
                                    parent,
                                    index + 1
                                );
                            }
                            TypeChecker.PutError("Comma placed outside of function call.", token.line, true);
                            Environment.Exit(-1);
                            return null;
                        }
                    case TokenType.TOKEN_EXPR:
                        {
                            if (parent.Class == NodeClass.call)
                            {
                                switch (tokens[index + 1].type)
                                {
                                    case TokenType.TOKEN_PLUS:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(token.value, token.line),
                                                new ConstantNode(tokens[index + 2].value, token.line),
                                                BinaryExpressionNode.t.ADD,
                                                token.line
                                             );
                                            ((parent as CallNode).Arguments as ScopeNode).AddChild(node);
                                            return parse_r(
                                                (parent as CallNode).Arguments,
                                                node,
                                                index + 3
                                            );
                                        }
                                    case TokenType.TOKEN_MINUS:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(token.value, token.line),
                                                new ConstantNode(tokens[index + 2].value, token.line),
                                                BinaryExpressionNode.t.SUB,
                                                token.line
                                            );
                                            ((parent as CallNode).Arguments as ScopeNode).AddChild(node);
                                            return parse_r(
                                                (parent as CallNode).Arguments,
                                                node,
                                                index + 3
                                            );
                                        }
                                    case TokenType.TOKEN_STAR:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(token.value, token.line),
                                                new ConstantNode(tokens[index + 2].value, token.line),
                                                BinaryExpressionNode.t.MUL,
                                                token.line
                                            );
                                            ((parent as CallNode).Arguments as ScopeNode).AddChild(node);
                                            return parse_r(
                                                (parent as CallNode).Arguments,
                                                node,
                                                index + 3
                                            );
                                        }
                                    case TokenType.TOKEN_SLASH:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(token.value, token.line),
                                                new ConstantNode(tokens[index + 2].value, token.line),
                                                BinaryExpressionNode.t.DIV,
                                                token.line
                                            );
                                            ((parent as CallNode).Arguments as ScopeNode).AddChild(node);
                                            return parse_r(
                                                (parent as CallNode).Arguments,
                                                node,
                                                index + 3
                                            );
                                        }
                                    case TokenType.TOKEN_COMMA:
                                        {
                                            ConstantNode node = new ConstantNode(token.value, token.line);
                                            ((parent as CallNode).Arguments as ScopeNode).AddChild(node);
                                            return parse_r(
                                                root,
                                                parent,
                                                index + 2
                                            );
                                        }
                                    case TokenType.TOKEN_CPAREN:
                                        {
                                            (parent as CallNode).Arguments.AddChild(new ConstantNode(token.value, token.line));
                                            (root as ScopeNode).AddChild(parent);
                                            return parse_r(
                                                root,
                                                root,
                                                index + 2
                                            );
                                        }
                                    default:
                                        {
                                            TypeChecker.PutError($"Unexpected token after Expression: {tokens[index + 1]}.", tokens[index + 1].line, true);
                                            return null;
                                        }
                                }
                            }
                            if (parent.Class == NodeClass.assign)
                            {
                                switch (tokens[index + 1].type)
                                {
                                    case TokenType.TOKEN_PLUS:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(token.value, token.line),
                                                new ConstantNode(tokens[index + 2].value, token.line),
                                                BinaryExpressionNode.t.ADD,
                                                token.line
                                            );
                                            (parent as BinaryExpressionNode).right = node;
                                            (root as ScopeNode).AddChild(parent);
                                            return parse_r(
                                                root,
                                                node,
                                                index + 3
                                            );
                                        }
                                    case TokenType.TOKEN_MINUS:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(token.value, token.line),
                                                new ConstantNode(tokens[index + 2].value, token.line),
                                                BinaryExpressionNode.t.SUB,
                                                token.line
                                            );
                                            (parent as BinaryExpressionNode).right = node;
                                            (root as ScopeNode).AddChild(parent);
                                            return parse_r(
                                                root,
                                                node,
                                                index + 3
                                            );
                                        }
                                    case TokenType.TOKEN_STAR:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(token.value, token.line),
                                                new ConstantNode(tokens[index + 2].value, token.line),
                                                BinaryExpressionNode.t.MUL,
                                                token.line
                                            );
                                            (parent as BinaryExpressionNode).right = node;
                                            (root as ScopeNode).AddChild(node);
                                            return parse_r(
                                                root,
                                                node,
                                                index + 3
                                            );
                                        }
                                    case TokenType.TOKEN_SLASH:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(token.value, token.line),
                                                new ConstantNode(tokens[index + 2].value, token.line),
                                                BinaryExpressionNode.t.DIV,
                                                token.line
                                            );
                                            (parent as BinaryExpressionNode).right = node;
                                            (root as ScopeNode).AddChild(parent);
                                            return parse_r(
                                                root,
                                                node,
                                                index + 3
                                            );
                                        }
                                    case TokenType.TOKEN_SEMI:
                                        {
                                            ConstantNode node = new ConstantNode(token.value, token.line);
                                            (parent as BinaryExpressionNode).right = node;
                                            (root as ScopeNode).AddChild(parent);
                                            return parse_r(
                                                root,
                                                root,
                                                index + 1
                                            );
                                        }
                                    default:
                                        {
                                            TypeChecker.PutError($"Unexpected token after Expression: {tokens[index + 1]}.", tokens[index + 1].line, true);
                                            return null;
                                        }
                                }
                            }
                            if (parent.Class != NodeClass.var)
                            {
                                switch (tokens[index + 1].type)
                                {
                                    case TokenType.TOKEN_PLUS:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(token.value, token.line),
                                                new ConstantNode(tokens[index + 2].value, token.line),
                                                BinaryExpressionNode.t.ADD,
                                                token.line
                                             );
                                            (root as ScopeNode).AddChild(node);
                                            return parse_r(
                                                root,
                                                node,
                                                index + 3
                                            );
                                        }
                                    case TokenType.TOKEN_MINUS:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(token.value, token.line),
                                                new ConstantNode(tokens[index + 2].value, token.line),
                                                BinaryExpressionNode.t.SUB,
                                                token.line
                                            );
                                            (root as ScopeNode).AddChild(node);
                                            return parse_r(
                                                root,
                                                node,
                                                index + 3
                                            );
                                        }
                                    case TokenType.TOKEN_STAR:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(token.value, token.line),
                                                new ConstantNode(tokens[index + 2].value, token.line),
                                                BinaryExpressionNode.t.MUL,
                                                token.line
                                            );
                                            (root as ScopeNode).AddChild(node);
                                            return parse_r(
                                                root,
                                                node,
                                                index + 3
                                            );
                                        }
                                    case TokenType.TOKEN_SLASH:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(token.value, token.line),
                                                new ConstantNode(tokens[index + 2].value, token.line),
                                                BinaryExpressionNode.t.DIV,
                                                token.line
                                            );
                                            (root as ScopeNode).AddChild(node);
                                            return parse_r(
                                                root,
                                                node,
                                                index + 3
                                            );
                                        }
                                    case TokenType.TOKEN_SEMI:
                                        {
                                            ConstantNode node = new ConstantNode(token.value, token.line);
                                            (root as ScopeNode).AddChild(node);
                                            return parse_r(
                                                root,
                                                root,
                                                index + 1
                                            );
                                        }
                                    default:
                                        {
                                            TypeChecker.PutError($"Unexpected token after Expression: {tokens[index + 1]}.", tokens[index + 1].line, true);
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
                                                new ConstantNode(token.value, token.line),
                                                new ConstantNode(tokens[index + 2].value, token.line),
                                                BinaryExpressionNode.t.ADD,
                                                token.line
                                             );
                                            (parent as VariableNode).Data = node;
                                            return parse_r(root, node, index + 3);
                                        }
                                    case TokenType.TOKEN_MINUS:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(token.value, token.line),
                                                new ConstantNode(tokens[index + 2].value, token.line),
                                                BinaryExpressionNode.t.SUB,
                                                token.line
                                            );
                                            (parent as VariableNode).Data = node;
                                            return parse_r(root, node, index + 3);
                                        }
                                    case TokenType.TOKEN_STAR:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(token.value, token.line),
                                                new ConstantNode(tokens[index + 2].value, token.line),
                                                BinaryExpressionNode.t.MUL,
                                                token.line
                                            );
                                            (parent as VariableNode).Data = node;
                                            return parse_r(root, node, index + 3);
                                        }
                                    case TokenType.TOKEN_SLASH:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(token.value, token.line),
                                                new ConstantNode(tokens[index + 2].value, token.line),
                                                BinaryExpressionNode.t.DIV,
                                                token.line
                                            );
                                            (parent as VariableNode).Data = node;
                                            return parse_r(root, node, index + 3);
                                        }
                                    case TokenType.TOKEN_SEMI:
                                        {
                                            (parent as VariableNode).Data = new ConstantNode(tokens[index].value, token.line);
                                            return parse_r(root, root, index + 1);
                                        }
                                }
                            }
                            (root as ScopeNode).AddChild(new ConstantNode(token.value, token.line));
                            return parse_r(root, parent, index + 2);
                        }
                    case TokenType.TOKEN_MINUS:
                        {
                            if (index == 0)
                            {
                                TypeChecker.PutError($"Unexpected number of tokens.", tokens[index + 1].line, true);
                            }
                            if (tokens[index + 1].type == TokenType.TOKEN_EXPR && tokens[index - 1].type == TokenType.TOKEN_EXPR || tokens[index + 1].type == TokenType.TOKEN_ID)
                            {
                                BinaryExpressionNode node = new BinaryExpressionNode(
                                    (parent as BinaryExpressionNode).right,
                                    new ConstantNode(tokens[index + 1].value, token.line),
                                    (tokens[index + 1].type == TokenType.TOKEN_EXPR) ? BinaryExpressionNode.t.SUB : BinaryExpressionNode.t.VARSUB,
                                    token.line
                                );
                                (parent as BinaryExpressionNode).right = node;
                                return parse_r(root, node, index + 2);
                            }

                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Cannot add to type: {token.type} LN {token.line}");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Environment.Exit(-1);
                        }
                        break;
                    case TokenType.TOKEN_PLUS:
                        {
                            if (index == 0)
                            {
                                TypeChecker.PutError($"Unexpected number of tokens.", tokens[index + 1].line, true);
                            }
                            if (tokens[index + 1].type == TokenType.TOKEN_EXPR || tokens[index + 1].type == TokenType.TOKEN_ID)
                            {
                                BinaryExpressionNode node = new BinaryExpressionNode(
                                    (parent as BinaryExpressionNode).right,
                                    new ConstantNode(tokens[index + 1].value, token.line),
                                    (tokens[index + 1].type == TokenType.TOKEN_EXPR) ? BinaryExpressionNode.t.ADD : BinaryExpressionNode.t.VARADD,
                                    token.line
                                );
                                (parent as BinaryExpressionNode).right = node;
                                return parse_r(root, node, index + 2);
                            }
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Cannot add to type: {token.type} LN {token.line}");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Environment.Exit(-1);
                        }
                        break;
                    case TokenType.TOKEN_STAR:
                        {
                            if (tokens[index + 1].type == TokenType.TOKEN_EXPR || tokens[index + 1].type == TokenType.TOKEN_ID)
                            {
                                BinaryExpressionNode node = new BinaryExpressionNode(
                                    (parent as BinaryExpressionNode).right,
                                    new ConstantNode(tokens[index + 1].value, token.line),
                                    (tokens[index + 1].type == TokenType.TOKEN_EXPR) ? BinaryExpressionNode.t.MUL : BinaryExpressionNode.t.VARMUL,
                                    token.line
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
                        }
                        break;
                    case TokenType.TOKEN_SLASH:
                        {
                            if (index == 0)
                            {
                                TypeChecker.PutError($"Unexpected number of tokens.", tokens[index + 1].line, true);
                            }
                            if (tokens[index + 1].type == TokenType.TOKEN_EXPR || tokens[index + 1].type == TokenType.TOKEN_ID)
                            {
                                BinaryExpressionNode node = new BinaryExpressionNode(
                                    (parent as BinaryExpressionNode).right,
                                    new ConstantNode(tokens[index + 1].value, token.line),
                                    (tokens[index + 1].type == TokenType.TOKEN_EXPR) ? BinaryExpressionNode.t.DIV : BinaryExpressionNode.t.VARDIV,
                                    token.line
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
                        }
                        break;
                    case TokenType.TOKEN_SEMI:
                        {
                            if (index == 0)
                            {
                                TypeChecker.PutError($"Unexpected number of tokens.", tokens[index + 1].line, true);
                            }
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
                                    }
                                    break;
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
                            switch (tokens[index + 2].type)
                            {
                                case TokenType.TOKEN_INT:
                                    {
                                        ret = FunctionNode.ReturnTypes.@int;
                                    }
                                    break;
                            }
                            FunctionNode node = new FunctionNode(tokens[index + 1].value.ToString(), ret, token.line);
                            node.Root = root;
                            return parse_r(
                                node,
                                node,
                                index + 3
                            );
                        }
                    case TokenType.TOKEN_BEGIN:
                        {
                            if (parent.Class != NodeClass.function)
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
                            ScopeNode Root = root as ScopeNode;
                            if (root.Value != "GLOBAL" && root.Class != NodeClass.scope)
                            {
                                (Root.Root as ScopeNode).AddChild(root);
                                return parse_r(
                                    Root.Root,
                                    Root.Root,
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
                                    new ConstantNode(null, token.line),
                                    token.line
                                );
                                ((root as FunctionNode).Arguments as ScopeNode).AddChild(node);
                                return parse_r(root,
                                    parent,
                                    index + 2
                                );
                            }
                            if (root.Class == NodeClass.scope || root.Class == NodeClass.function)
                            {
                                if (index + 3 > tokens.Count)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($"Variable declaration enough arguments REQUIRED : VARTYPE NAME = DECLARATION OR NULL;. LN {token.line}");
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                    Environment.Exit(-1);
                                }
                                if (tokens[index + 1].type != TokenType.TOKEN_ID)
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
                                        new ConstantNode(null, token.line),
                                        token.line
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
                                        new ConstantNode(tokens[index + 3].value, token.line),
                                        token.line
                                    );
                                    (root as ScopeNode).AddChild(n);
                                    return parse_r(root, n, index + 4);
                                }
                                VariableNode node = new VariableNode(
                                    tokens[index + 1].value.ToString(),
                                    VariableNode.VariableClass.@byte,
                                    null,
                                    token.line
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
                    case TokenType.TOKEN_INT:
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
                                    VariableNode.VariableClass.@int,
                                    new ConstantNode(null, token.line),
                                    token.line
                                );
                                ((root as FunctionNode).Arguments as ScopeNode).AddChild(node);
                                return parse_r(root,
                                    parent,
                                    index + 2
                                );
                            }
                            if (root.Class == NodeClass.scope || root.Class == NodeClass.function)
                            {
                                if (index + 3 > tokens.Count)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($"Variable declaration enough arguments REQUIRED : VARTYPE NAME = DECLARATION OR NULL;. LN {token.line}");
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                    Environment.Exit(-1);
                                }
                                if (tokens[index + 1].type != TokenType.TOKEN_ID)
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
                                        VariableNode.VariableClass.@int,
                                        new ConstantNode(null, token.line),
                                        token.line
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
                                        VariableNode.VariableClass.@int,
                                        new ConstantNode(tokens[index + 3].value, token.line),
                                        token.line
                                    );
                                    (root as ScopeNode).AddChild(n);
                                    return parse_r(root, n, index + 4);
                                }
                                VariableNode node = new VariableNode(
                                    tokens[index + 1].value.ToString(),
                                    VariableNode.VariableClass.@int,
                                    null,
                                    token.line
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
                                null,
                                token.line
                            );
                            if (root.Class != NodeClass.function)
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
                    case TokenType.TOKEN_ID:
                        {
                            if (root.Class == NodeClass.function)
                            {
                                if ((root as FunctionNode).ReturnDefine)
                                {
                                    switch (tokens[index + 1].type)
                                    {
                                        case TokenType.TOKEN_PLUS:
                                            {
                                                BinaryExpressionNode node = new BinaryExpressionNode(
                                                    new ConstantNode(
                                                        token.value,
                                                        token.line
                                                    ),
                                                    new ConstantNode(
                                                        tokens[index + 2].value,
                                                        token.line
                                                    ),
                                                    BinaryExpressionNode.t.VARADD,
                                                    token.line
                                                );
                                                ((root as FunctionNode).ReturnValue as VariableNode).Data = node;
                                                return parse_r(root, node, index + 3);
                                            }
                                        case TokenType.TOKEN_MINUS:
                                            {
                                                BinaryExpressionNode node = new BinaryExpressionNode(
                                                    new ConstantNode(
                                                        token.value,
                                                        token.line
                                                    ),
                                                    new ConstantNode(
                                                        tokens[index + 2].value,
                                                        token.line
                                                    ),
                                                    BinaryExpressionNode.t.VARSUB,
                                                    token.line
                                                );
                                                ((root as FunctionNode).ReturnValue as VariableNode).Data = node;
                                                return parse_r(root, node, index + 3);
                                            }
                                        case TokenType.TOKEN_STAR:
                                            {
                                                BinaryExpressionNode node = new BinaryExpressionNode(
                                                    new ConstantNode(
                                                        token.value,
                                                        token.line
                                                    ),
                                                    new ConstantNode(
                                                        tokens[index + 2].value,
                                                        token.line
                                                    ),
                                                    BinaryExpressionNode.t.VARMUL,
                                                    token.line
                                                );
                                                ((root as FunctionNode).ReturnValue as VariableNode).Data = node;
                                                return parse_r(root, node, index + 3);
                                            }
                                        case TokenType.TOKEN_SLASH:
                                            {
                                                BinaryExpressionNode node = new BinaryExpressionNode(
                                                    new ConstantNode(
                                                        token.value,
                                                        token.line
                                                    ),
                                                    new ConstantNode(
                                                        tokens[index + 2].value,
                                                        token.line
                                                    ),
                                                    BinaryExpressionNode.t.VARDIV,
                                                    token.line
                                                );
                                                ((root as FunctionNode).ReturnValue as VariableNode).Data = node;
                                                return parse_r(root, node, index + 3);
                                            }
                                    }
                                }
                            }
                            if (parent.Class == NodeClass.var)
                            {
                                switch (tokens[index + 1].type)
                                {
                                    case TokenType.TOKEN_PLUS:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(
                                                    token.value,
                                                    token.line
                                                ),
                                                new ConstantNode(
                                                    tokens[index + 2].value,
                                                    token.line
                                                ),
                                                BinaryExpressionNode.t.VARADD,
                                                token.line
                                            );
                                            (parent as VariableNode).Data = node;
                                            return parse_r(root, node, index + 3);
                                        }
                                    case TokenType.TOKEN_MINUS:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(
                                                    token.value,
                                                    token.line
                                                ),
                                                new ConstantNode(
                                                    tokens[index + 2].value,
                                                    token.line
                                                ),
                                                BinaryExpressionNode.t.VARSUB,
                                                token.line
                                            );
                                            (parent as VariableNode).Data = node;
                                            return parse_r(root, node, index + 3);
                                        }
                                    case TokenType.TOKEN_STAR:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(
                                                    token.value,
                                                    token.line
                                                ),
                                                new ConstantNode(
                                                    tokens[index + 2].value,
                                                    token.line
                                                ),
                                                BinaryExpressionNode.t.VARMUL,
                                                token.line
                                            );
                                            (parent as VariableNode).Data = node;
                                            return parse_r(root, node, index + 3);
                                        }
                                    case TokenType.TOKEN_SLASH:
                                        {
                                            BinaryExpressionNode node = new BinaryExpressionNode(
                                                new ConstantNode(
                                                    token.value,
                                                    token.line
                                                ),
                                                new ConstantNode(
                                                    tokens[index + 2].value,
                                                    token.line
                                                ),
                                                BinaryExpressionNode.t.VARDIV,
                                                token.line
                                            );
                                            (parent as VariableNode).Data = node;
                                            return parse_r(root, node, index + 3);
                                        }
                                    case TokenType.TOKEN_BOOLEQ:
                                        {
                                            if (tokens[index + 2].type == TokenType.TOKEN_ID)
                                            {
                                                BinaryExpressionNode node = new BinaryExpressionNode(
                                                    new ConstantNode(token.value, token.line),
                                                    new ConstantNode(tokens[index + 2].value, token.line),
                                                        BinaryExpressionNode.t.BOOLEQ,
                                                    token.line
                                                );
                                                (parent as VariableNode).Data = node;
                                                return parse_r(
                                                    root,
                                                    node,
                                                    index + 3
                                                );
                                            }
                                            else
                                            {
                                                BinaryExpressionNode node = new BinaryExpressionNode(
                                                    new ConstantNode(token.value, token.line),
                                                    null,
                                                    BinaryExpressionNode.t.BOOLEQ,
                                                    token.line
                                                );
                                                return parse_r(
                                                    root,
                                                    node,
                                                    index + 2
                                                );
                                            }
                                        }
                                    case TokenType.TOKEN_MORE:
                                        {
                                            if (tokens[index + 2].type == TokenType.TOKEN_ID)
                                            {
                                                BinaryExpressionNode node = new BinaryExpressionNode(
                                                    new ConstantNode(token.value, token.line),
                                                    new ConstantNode(tokens[index + 2].value, token.line),
                                                        BinaryExpressionNode.t.BOOLMORE,
                                                    token.line
                                                );
                                                (parent as VariableNode).Data = node;
                                                return parse_r(
                                                    root,
                                                    node,
                                                    index + 3
                                                );
                                            }
                                            else
                                            {
                                                BinaryExpressionNode node = new BinaryExpressionNode(
                                                    new ConstantNode(token.value, token.line),
                                                    null,
                                                    BinaryExpressionNode.t.BOOLEQ,
                                                    token.line
                                                );
                                                return parse_r(
                                                    root,
                                                    node,
                                                    index + 2
                                                );
                                            }
                                        }
                                    case TokenType.TOKEN_LESS:
                                        {
                                            if (tokens[index + 2].type == TokenType.TOKEN_ID)
                                            {
                                                BinaryExpressionNode node = new BinaryExpressionNode(
                                                    new ConstantNode(token.value, token.line),
                                                    new ConstantNode(tokens[index + 2].value, token.line),
                                                        BinaryExpressionNode.t.BOOLLESS,
                                                    token.line
                                                );
                                                (parent as VariableNode).Data = node;
                                                return parse_r(
                                                    root,
                                                    node,
                                                    index + 3
                                                );
                                            }
                                            else
                                            {
                                                BinaryExpressionNode node = new BinaryExpressionNode(
                                                    new ConstantNode(token.value, token.line),
                                                    null,
                                                    BinaryExpressionNode.t.BOOLEQ,
                                                    token.line
                                                );
                                                return parse_r(
                                                    root,
                                                    node,
                                                    index + 2
                                                );
                                            }
                                        }
                                }
                            }
                            switch (tokens[index + 1].type)
                            {
                                case TokenType.TOKEN_PLUS:
                                    {
                                        BinaryExpressionNode node = new BinaryExpressionNode(
                                            new ConstantNode(
                                                token.value,
                                                token.line
                                            ),
                                            new ConstantNode(
                                                tokens[index + 2].value,
                                                token.line
                                            ),
                                            BinaryExpressionNode.t.VARADD,
                                            token.line
                                        );
                                        (root as ScopeNode).AddChild(node);
                                        return parse_r(root, node, index + 3);
                                    }
                                case TokenType.TOKEN_MINUS:
                                    {
                                        BinaryExpressionNode node = new BinaryExpressionNode(
                                            new ConstantNode(
                                                token.value,
                                                token.line
                                            ),
                                            new ConstantNode(
                                                tokens[index + 2].value,
                                                token.line
                                            ),
                                            BinaryExpressionNode.t.VARSUB,
                                            token.line
                                        );
                                        (root as ScopeNode).AddChild(node);
                                        return parse_r(root, node, index + 3);
                                    }
                                case TokenType.TOKEN_STAR:
                                    {
                                        BinaryExpressionNode node = new BinaryExpressionNode(
                                            new ConstantNode(
                                                token.value,
                                                token.line
                                            ),
                                            new ConstantNode(
                                                tokens[index + 2].value,
                                                token.line
                                            ),
                                            BinaryExpressionNode.t.VARMUL,
                                            token.line
                                        );
                                        (root as ScopeNode).AddChild(node);
                                        return parse_r(root, node, index + 3);
                                    }
                                case TokenType.TOKEN_SLASH:
                                    {
                                        BinaryExpressionNode node = new BinaryExpressionNode(
                                            new ConstantNode(
                                                token.value,
                                                token.line
                                            ),
                                            new ConstantNode(
                                                tokens[index + 2].value,
                                                token.line
                                            ),
                                            BinaryExpressionNode.t.VARDIV,
                                            token.line
                                        );
                                        (root as ScopeNode).AddChild(node);
                                        return parse_r(root, node, index + 3);
                                    }
                            }
                            if (tokens[index + 1].type == TokenType.TOKEN_OPAREN)
                            {
                                CallNode node = new CallNode(token.value.ToString(), token.line);
                                return parse_r(root, node, index + 2);
                            }
                            if (root.Class == NodeClass.function)
                            {
                                if ((root as FunctionNode).ReturnDefine)
                                {
                                    ((root as FunctionNode).ReturnValue as VariableNode).Data = new ConstantNode(token.value, token.line);
                                    return parse_r(
                                        root,
                                        root,
                                        index + 1
                                    );
                                }
                            }
                            if (tokens[index + 1].type == TokenType.TOKEN_BOOLEQ)
                            {
                                if (tokens[index + 2].type == TokenType.TOKEN_ID)
                                {
                                    BinaryExpressionNode node = new BinaryExpressionNode(
                                        new ConstantNode(token.value, token.line),
                                        new ConstantNode(tokens[index + 2].value, token.line),
                                            BinaryExpressionNode.t.BOOLEQ,
                                        token.line
                                    );
                                    (root as ScopeNode).AddChild(node);
                                    return parse_r(
                                        root,
                                        node,
                                        index + 3
                                    );
                                }
                                else
                                {
                                    BinaryExpressionNode node = new BinaryExpressionNode(
                                        new ConstantNode(token.value, token.line),
                                        null,
                                        BinaryExpressionNode.t.BOOLEQ,
                                        token.line
                                    );
                                    return parse_r(
                                        root,
                                        node,
                                        index + 2
                                    );
                                }
                            }
                            if (tokens[index + 1].type == TokenType.TOKEN_LESS)
                            {
                                if (tokens[index + 2].type == TokenType.TOKEN_ID)
                                {
                                    BinaryExpressionNode node = new BinaryExpressionNode(
                                        new ConstantNode(token.value, token.line),
                                        new ConstantNode(tokens[index + 2].value, token.line),
                                         BinaryExpressionNode.t.BOOLLESS,
                                        token.line
                                    );
                                    (root as ScopeNode).AddChild(node);
                                    return parse_r(
                                        root,
                                        node,
                                        index + 3
                                    );
                                }
                                else
                                {
                                    BinaryExpressionNode node = new BinaryExpressionNode(
                                        new ConstantNode(token.value, token.line),
                                        null,
                                        BinaryExpressionNode.t.BOOLLESS,
                                        token.line
                                    );
                                    return parse_r(
                                        root,
                                        node,
                                        index + 2
                                    );
                                }
                            }
                            if (tokens[index + 1].type == TokenType.TOKEN_MORE)
                            {
                                if (tokens[index + 2].type == TokenType.TOKEN_ID)
                                {
                                    BinaryExpressionNode node = new BinaryExpressionNode(
                                        new ConstantNode(token.value, token.line),
                                        new ConstantNode(tokens[index + 2].value, token.line),
                                         BinaryExpressionNode.t.BOOLMORE,
                                        token.line
                                    );
                                    (root as ScopeNode).AddChild(node);
                                    return parse_r(
                                        root,
                                        node,
                                        index + 3
                                    );
                                }
                                else
                                {
                                    BinaryExpressionNode node = new BinaryExpressionNode(
                                        new ConstantNode(token.value, token.line),
                                        null,
                                        BinaryExpressionNode.t.BOOLMORE,
                                        token.line
                                    );
                                    return parse_r(
                                        root,
                                        node,
                                        index + 2
                                    );
                                }
                            }
                            if (tokens[index + 1].type == TokenType.TOKEN_EQ)
                            {
                                if (tokens[index + 2].type == TokenType.TOKEN_ID)
                                {
                                    BinaryExpressionNode node = new BinaryExpressionNode(
                                        new ConstantNode(token.value, token.line),
                                        new ConstantNode(tokens[index + 2].value, token.line),
                                        BinaryExpressionNode.t.ASSIGN,
                                        token.line
                                    );
                                    (root as ScopeNode).AddChild(node);
                                    return parse_r(
                                        root,
                                        node,
                                        index + 3
                                    );
                                }
                                else
                                {
                                    BinaryExpressionNode node = new BinaryExpressionNode(
                                        new ConstantNode(token.value, token.line),
                                        null,
                                        BinaryExpressionNode.t.ASSIGN,
                                        token.line
                                    );
                                    return parse_r(
                                        root,
                                        node,
                                        index + 2
                                    );
                                }
                            }
                            return null;
                        }
                    case TokenType.TOKEN_CPAREN:
                        {
                            (root as ScopeNode).AddChild(parent);
                            return parse_r(
                                root,
                                root,
                                index + 2
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
            return parse_r(new ScopeNode("GLOBAL", 0), new ScopeNode("GLOBAL", 0), 0);
        }
    }
}
