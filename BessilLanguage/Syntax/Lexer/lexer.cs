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
                while(current != '\n')
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
                    case "short": // 16 bit
                        {
                            return new Token(TokenType.TOKEN_SHORT, data, line);
                        }
                    case "long": // 64 bit
                        {
                            return new Token(TokenType.TOKEN_LONG, data, line);
                        }
                    case "null":
                    case "NULL":
                    case "nill":
                        {
                            return new Token(TokenType.TOKEN_NULL, null, line);
                        }
                    case "int":  // 32 bit
                        {
                            return new Token(TokenType.TOKEN_INT, data, line);
                        }
                    case "return":
                        {
                            return new Token(TokenType.TOKEN_RETURN, data, line);
                        }
                    case "asm":
                        {
                            int s = position;
                            while (current != ';')
                            {
                                next();
                            }
                            int e = position;
                            next();
                            return new Token(TokenType.TOKEN_ASM, source.Substring(s, e - s), line);
                        }
                    case "extern":
                        {
                            int s = position;
                            while (current != ';' && current != '\n')
                            {
                                next();
                            }
                            int e = position;
                            next();
                            return new Token(TokenType.TOKEN_EXTERN, source.Substring(s, e - s), line);
                        }
                    case "if":
                        {
                            return new Token(TokenType.TOKEN_IF, data, line);
                        }
                }
                return new Token(TokenType.TOKEN_ID, data, line);
            }
            if (char.IsDigit(current))
            {
                int start = position;
                int nx = 0;
                while(char.IsDigit(current) || current == 'x' || (( current >= 'A' && current <= 'F' || current >= 'a' && current <= 'f') && nx > 0 ))
                {
                    if(current == 'x')
                    {
                        ++nx;
                        if(nx > 1)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Lexer expected Token: EXPR. The lexer was given was Token: EXPR Token: ID, letter was placed within expression. LN : {line}");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Environment.Exit(-1);
                        }
                    }
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
                if(nx > 0)
                {
                    return new Token(TokenType.TOKEN_EXPR, long.Parse( source.Substring(start, end - start).Replace("0x", ""), System.Globalization.NumberStyles.HexNumber), line);
                }
                return new Token(TokenType.TOKEN_EXPR, long.Parse(source.Substring(start, end - start)), line);
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
                        if (position - 2 > 0)
                        {

                            if (char.IsDigit(current) && !char.IsDigit(source[position - 2]))
                            {
                                int start = position - 1;
                                while (char.IsDigit(current))
                                {
                                    next();
                                }
                                int end = position;
                                return new Token(TokenType.TOKEN_EXPR, int.Parse(source.Substring(start, end - start)), line);
                            }
                        }
                        else {
                            if (char.IsDigit(current))
                            {
                                int start = position - 1;
                                while (char.IsDigit(current))
                                {
                                    next();
                                }
                                int end = position;
                                return new Token(TokenType.TOKEN_EXPR, int.Parse(source.Substring(start, end - start)), line);
                            }
                        }
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
                        if(current == '=')
                        {
                            next();
                            return new Token(TokenType.TOKEN_BOOLEQ, "==", line);
                        }
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
                        return new Token(TokenType.TOKEN_BEGIN, "{", line);
                    }
                case '}':
                    {
                        next();
                        return new Token(TokenType.TOKEN_END, "}", line);
                    }
                case ',':
                    {
                        next();
                        return new Token(TokenType.TOKEN_COMMA, ",", line);
                    }
                case '>':
                    {
                        next();
                        if(current == '=')
                        {
                            next();
                            return new Token(TokenType.TOKEN_MORE_EQ, ">=" ,line);
                        }
                        return new Token(TokenType.TOKEN_MORE, ">", line);
                    }
                case '<':
                    {
                        next();
                        if (current == '=')
                        {
                            next();
                            return new Token(TokenType.TOKEN_LESS_EQ, "<=", line);
                        }
                        return new Token(TokenType.TOKEN_LESS, ">", line);
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
