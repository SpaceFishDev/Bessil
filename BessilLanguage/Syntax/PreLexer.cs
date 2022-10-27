using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BessilLanguage
{
    internal class PreLexer
    {
        public static string includes(string source)
        {
            source = source.Replace("\r\n", "\n");
            int i = 0;
            List<string> s = source.Split("\n").ToList();
            foreach(string line in s)
            {
                if (line.StartsWith("%i "))
                {
                    string src = "";
                    string path = line.Replace("%i ", "");
                    try
                    {
                        src = File.ReadAllText(path);
                    }
                    catch
                    {
                        Console.WriteLine($"Could not read source file: {path}");
                        Environment.Exit(-1);
                    }
                    source = source.Replace(line + "\n", src);
                    return includes(source);
                }
            }
            Console.WriteLine(source);
            return source;
        }
    }
}
