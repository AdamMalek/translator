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
            Console.ForegroundColor = consoleColor;
            return true;
        }
        int r = -1;
        ConsoleColor[] colors = (Enum.GetValues(typeof(ConsoleColor)) as ConsoleColor[]).Where(x=> x != ConsoleColor.Red && x != ConsoleColor.Black && x != ConsoleColor.Blue && x != ConsoleColor.DarkBlue).ToArray();

        // osobna funkcja, zeby obsluzyc EOF
        private void W()
        {
            X();
            AcceptCharacter(SymbolType.EOF);
        }
        /*
            X -> (X)
            X -> (X) + X
            X -> d + X
            X -> d
            
            d-> dane (liczba int, float lub zmienna)
            + -> operacja matematyczna [+-/*] 
        */
        private void X()
        {
            r++;
            if (_currentSymbol.Type == SymbolType.LeftBracket)
            {
                AcceptCharacter(SymbolType.LeftBracket);
                X();
                AcceptCharacter(SymbolType.RightBracket);
                if (_currentSymbol.Type == SymbolType.MathSymbol)
                {
                    AcceptCharacter(SymbolType.MathSymbol);
                    X();
                }
            }
            else
            {
                AcceptCharacter(SymbolType.Data);
                if (_currentSymbol.Type == SymbolType.MathSymbol)
                {
                    AcceptCharacter(SymbolType.MathSymbol);
                    X();
                }
            }
            r--;
        }
        void AcceptCharacter(SymbolType expectedType)
        {
            Console.ForegroundColor = colors[((r>0)? r:0) % colors.Length];
            if (_currentSymbol.Type == expectedType)
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
                throw new InvalidSyntaxException(_currentSymbol.Position, _currentSymbol.SymbolString, null);
            }
        }
    }
}
