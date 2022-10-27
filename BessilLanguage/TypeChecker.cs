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
    }
}
