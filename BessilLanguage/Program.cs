namespace BessilLanguage
{
    class Program
    {
        static void Main(string[] Args)
        {
            Lexer lexer = new Lexer("1 + 1;");
            while (true)
            {
                Token token = lexer.lex();
                Console.WriteLine($"TOKEN({token.type}, {token.value})");
                if(token.type == TokenType.TOKEN_ENDOFFILE)
                {
                    return;
                }
            }
        }
    }
}