using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BessilLanguage
{
    public class Lexer
    {
        public int position;
        public string source;
        public char current => (position < source.Length) ? source[position] : '\0';
        public void next()
        {
            ++position; 
        }
        public int line = 1;
        public Token lex()
        {
            if( current == '\0')
            {
                return new Token(TokenType.TOKEN_ENDOFFILE, "\0", line);
            }
            if (current == '#')
            {
                next();
                return lex();
            }
            if (char.IsLetter(current))
            {
                int start = position;
                while (char.IsLetter(current) || current == '_' || char.IsDigit(current))
                {
                    next();
                }
                int end = position;
                string data = source.Substring(start, end - start);
                switch (data)
                {
                    case "func":
                        {
                            return new Token(TokenType.TOKEN_FUNC, data, line);
                        }
                    case "char":
                    case "byte": // 8 bit
                        {
                            return new Token(TokenType.TOKEN_BYTE, data, line);
                        }
                    case "lint":
                    case "long_int": // 64 bit
                        {
                            return new Token(TokenType.TOKEN_LONG, data, line);
                        }
                    case "int":  // 32 bit
                        {
                            return new Token(TokenType.TOKEN_INT, data, line);
                        }
                }
                return new Token(TokenType.TOKEN_ID, data, line);
            }
            if (char.IsDigit(current))
            {
                int start = position;
                while(char.IsDigit(current))
                {
                    next();
                }
                if (char.IsLetter(current))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Lexer expected Token: EXPR. The lexer was given was Token: EXPR Token: ID, letter was placed within expression. LN : {line}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Environment.Exit(-1);
                }
                int end = position;
                return new Token(TokenType.TOKEN_EXPR, int.Parse(source.Substring(start, end - start)), line);
            }
            if(current == '\n')
            {
                ++line;
                next();
                return lex();
            }
            if (char.IsWhiteSpace(current))
            {
                next();
                return lex();
            }
            switch (current)
            {
                case ';':
                    {
                        next();
                        return new Token(TokenType.TOKEN_SEMI, ";", line); 
                    }
                case '+':
                    {
                        next();
                        return new Token(TokenType.TOKEN_PLUS, "+", line);
                    }
                case '-':
                    {
                        next();
                        return new Token(TokenType.TOKEN_MINUS, "-", line);
                    }
                case '*':
                    {
                        next();
                        return new Token(TokenType.TOKEN_STAR, "*", line);
                    }
                case '/':
                    {
                        next();
                        return new Token(TokenType.TOKEN_SLASH, "/", line);
                    }
                case '\'':
                case '"':
                    {
                        next(); 
                        int start = position;
                        while(current != '"' && current != '\'')
                        {
                            next();
                        }
                        int end = position;
                        next();
                        return new Token(TokenType.TOKEN_STRING, source.Substring(start, end - start), line);
                    }
                case '=':
                    {
                        next();
                        return new Token(TokenType.TOKEN_EQ, "=", line);
                    }
                case '(':
                    {
                        next();
                        return new Token(TokenType.TOKEN_OPAREN, "(", line);
                    }
                case ')':
                    {
                        next();
                        return new Token(TokenType.TOKEN_CPAREN, ")", line);
                    }
                case '{':
                    {
                        next();
                        return new Token(TokenType.TOKEN_OBRACKET, "{", line);
                    }
                case '}':
                    {
                        next();
                        return new Token(TokenType.TOKEN_CBRACKET, "}", line);
                    }
            }
            Console.WriteLine($"Bad character in input: {current}");
            Environment.Exit(-1);
            return new Token(TokenType.BAD_TOKEN, current.ToString(), line);
        }
        public Lexer(string source)
        {
            this.position = 0;
            this.source = source;
        }
    }
}
