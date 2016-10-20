using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using translator.Models;

namespace translator
{
    public class InvalidSyntaxException : ApplicationException
    {
        private string character;
        private SymbolType? expected;

        public int Position { get; set; }
        public Symbol[] Symbols { get; set; }
        public override string Message
        {
            get
            {
                var s1 = $"Error in position: { Position }, near character {character}";
                if (expected != null)
                    s1 += ", expected: " + getMessage(expected.Value);
                return s1;
            }
        }

        private string getMessage(SymbolType expected)
        {
            switch (expected)
            {
                case SymbolType.EOF:
                    return "End of File";
                case SymbolType.Data:
                    return "number or variable";
                case SymbolType.Number:
                    return "number";
                case SymbolType.Variable:
                    return "variable";
                case SymbolType.Integer:
                    return "integer";
                case SymbolType.Float:
                    return "float";
                case SymbolType.AddSubtract:
                    return "+ or -";
                case SymbolType.MultiplyDivide:
                    return "* or /";
                case SymbolType.LeftBracket:
                    return "(";
                case SymbolType.RightBracket:
                    return ")";
                default:
                    return "character";
            }
        }
        public InvalidSyntaxException(int pos, string character, Symbol[] symbols, SymbolType? expected = null)
        {
            this.expected = expected;
            Symbols = symbols;
            Position = pos;
            this.character = character;
        }
    }
}
