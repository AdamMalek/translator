using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using translator.Models;

namespace translator
{
    class Program
    {
        static void Main(string[] args)
        {
            var consoleColor = Console.ForegroundColor;
            //if (args.Length < 1)
            //{
            //    Console.WriteLine("usage: tr.exe [source_file_path]");
            //    Console.ReadLine();
            //    Environment.Exit(0);
            //}
            //string path = args[0];

            string path = "source.src";

            string src = File.ReadAllText(path);
            src = src.Replace("\r", "");
            Lexer lexer = new Lexer();
            Analyzer analyzer;
            Symbol[] symbols = new Symbol[] { };
            Console.WriteLine("Result:");
            Console.WriteLine("-----------------------");
            try
            {
                symbols = lexer.AnalizeSource(src);
                analyzer = new Analyzer(lexer);
                analyzer.Analyze();
                Console.WriteLine();
                Console.WriteLine("Success");
                Console.WriteLine("-----------------------");
            }
            catch (InvalidSyntaxException e)
            {
                Console.ForegroundColor = consoleColor;
                Console.WriteLine();
                Console.WriteLine(e.Message);
                Console.WriteLine();
                GetFaultFragment(e.Position, src);
                if (e.Symbols != null)
                {
                    foreach (var symbol in e.Symbols)
                    {
                        Console.WriteLine(DisplaySymbol(symbol));
                    }
                }
            }
            foreach (var symbol in symbols)
            {
                Console.WriteLine(DisplaySymbol(symbol));
            }

            Console.ReadLine();
        }

        private static string DisplaySymbol(Symbol symbol)
        {
            switch (symbol.Type)
            {
                case SymbolType.Variable:
                    return "variable: " + symbol.Displayname;
                case SymbolType.Integer:
                    return "int: " + symbol.Displayname;
                case SymbolType.Float:
                    return "float: " + symbol.Displayname;
                case SymbolType.MathSymbol:
                    return "math symbol: " + symbol.Displayname;
                case SymbolType.LeftBracket:
                    return "left bracket: " + symbol.Displayname;
                case SymbolType.RightBracket:
                    return "right bracket: " + symbol.Displayname;
                case SymbolType.Whitespace:
                    return "whitespace: " + symbol.Displayname;
                case SymbolType.Unknown:
                    return "unknown: " + symbol.Displayname;
                default:
                    return symbol.Displayname;
            }
        }

        static void GetFaultFragment(int pos, string code)
        {
            int lastenter;
            try
            {
                lastenter = code.Select((x, i) => new { item = x, index = i }).Last(xx => xx.item == '\n' && xx.index < pos).index;
            }
            catch (Exception)
            {
                lastenter = -1;
            }
            int firstenter;
            try
            {
                firstenter = code.Select((x, i) => new { item = x, index = i }).First(xx => xx.item == '\n' && xx.index > pos).index;
            }
            catch (Exception)
            {
                firstenter = code.Length;
            }
            Console.Write(code.Substring(lastenter + 1, pos - lastenter - 1));
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            var character = code.ElementAt(pos);
            if (string.IsNullOrWhiteSpace(character.ToString()))
            {
                Console.Write("_");
            }
            else
            {
                Console.Write(character);
            }
            Console.ForegroundColor = consoleColor;
            if (pos < code.Length - 1)
                Console.WriteLine(code.Substring(pos + 1, firstenter - pos - 1));
            Console.WriteLine();
        }
    }
}
