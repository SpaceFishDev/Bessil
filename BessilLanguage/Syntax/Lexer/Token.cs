using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BessilLanguage
{
    public enum TokenType
    {
		TOKEN_ID,
		TOKEN_EXPR,
		TOKEN_PLUS,
		TOKEN_MINUS,
		TOKEN_STAR,
		TOKEN_SLASH,
        TOKEN_BOOLEQ,
        TOKEN_MORE,
        TOKEN_LESS,
		TOKEN_STRING,
		TOKEN_ENDOFFILE,
		TOKEN_OPAREN,
		TOKEN_CPAREN,
		TOKEN_BEGIN,
		TOKEN_END,
		TOKEN_SEMI,
		TOKEN_EQ,
		TOKEN_FUNC,
		TOKEN_COMMA,
		TOKEN_BYTE,
		TOKEN_INT,
		TOKEN_LONG,
		BAD_TOKEN,
        TOKEN_NULL,
        TOKEN_RETURN,
        TOKEN_MORE_EQ,
        TOKEN_LESS_EQ,
        TOKEN_SHORT,
    }
    public class Token
    {
        public TokenType type;
        public object value;
		public int line;
        public Token(TokenType type, object value, int line)
        {
            this.type = type;
            this.value = value;
            this.line = line;
        }
    }
}
