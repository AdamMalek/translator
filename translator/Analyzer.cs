using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using translator.Models;

namespace translator
{
    public class Analyzer
    {
        private Lexer _lexer;
        private Symbol _currentSymbol;
        string res = "";
        public Analyzer(Lexer lexer)
        {
            _lexer = lexer;
            _currentSymbol = _lexer.GetNext();
        }
        // instead of using integer or float or variable use Data!
        public bool Analyze()
        {
            var consoleColor = Console.ForegroundColor;
            W();
            AcceptCharacter(SymbolType.EOF);
            Console.WriteLine();
            Console.WriteLine("Odwrotna notacja polska:");
            Console.WriteLine(res);
            Console.ForegroundColor = consoleColor;
            return true;
        }
        int r = -1;
        ConsoleColor[] colors = (Enum.GetValues(typeof(ConsoleColor)) as ConsoleColor[]).Where(x=> x != ConsoleColor.Red && x != ConsoleColor.Black && x != ConsoleColor.Blue && x != ConsoleColor.DarkBlue).ToArray();

        // osobna funkcja, zeby obsluzyc EOF
        private void W()
        {
            r++; // do wcięć w wyświetlaniu

            S();
            Wprim();

            r--;
        }

        private void alfa()
        {
            var s = _currentSymbol.SymbolString;
            AcceptCharacter(SymbolType.AddSubtract);
            S();
            res += s + " ";
        }
        private void Wprim()
        {
            if (_currentSymbol.Type == SymbolType.AddSubtract)
            {
                alfa();
                Wprim();
            }
        }
        private void S()
        {
            C();
            Sprim();
        }
        private void beta()
        {
            var s = _currentSymbol.SymbolString;
            AcceptCharacter(SymbolType.MultiplyDivide);
            C();
            res += s + " ";
        }
        private void Sprim()
        {
            if (_currentSymbol.Type == SymbolType.MultiplyDivide)
            {
                beta();
                Sprim();
            }
        }
        private void C()
        {
            if (_currentSymbol.Type == SymbolType.LeftBracket)
            {
                AcceptCharacter(SymbolType.LeftBracket);
                W();
                AcceptCharacter(SymbolType.RightBracket);
            }
            else
            {
                var s = _currentSymbol.SymbolString;
                AcceptCharacter(SymbolType.Data);
                res += s + " ";
            }
        }

        void AcceptCharacter(SymbolType expectedType)
        {
            Console.ForegroundColor = colors[((r>0)? r:0) % colors.Length];
            if (getType(_currentSymbol.Type) == expectedType)
            {
                for (int i = 0; i < r; i++)
                {
                    Console.Write("   ");
                }
                Console.WriteLine("" + _currentSymbol.SymbolString + "");
                _currentSymbol = _lexer.GetNext();
            }
            else
            {
                var consoleColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                for (int i = 0; i < r; i++)
                {
                    Console.Write("   ");
                }
                Console.WriteLine("" + _currentSymbol.SymbolString + "");
                Console.ForegroundColor = consoleColor;
                throw new InvalidSyntaxException(_currentSymbol.Position, _currentSymbol.SymbolString, null,expectedType);
            }
        }

        public SymbolType getType(SymbolType s)
        {
            return (s == SymbolType.Integer || s == SymbolType.Float || s == SymbolType.Variable) ? SymbolType.Data : s;
        }
    }
}
