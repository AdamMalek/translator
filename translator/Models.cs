using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace translator.Models
{
    public class Symbol
    {
        public SymbolType Type { get; set; }
        public string SymbolString { get; set; }
        public string Displayname
        {
            get
            {
                if (Type != SymbolType.Whitespace)
                {
                    return SymbolString;
                }
                else
                {
                    switch (SymbolString)
                    {
                        case "\t": return "Tabulator";
                        case " ": return "space";
                        case "\r\n": return "NewLine";
                        default: return SymbolString;
                    }
                }
            }
        }

        public int Position { get; set; } = 0;

    }
    public enum SymbolType
    {
        EOF,
        Data,
        Number,
        Empty,
        Variable,
        Integer,
        Float,
        AddSubtract,
        MultiplyDivide,
        LeftBracket,
        RightBracket,
        Whitespace,
        Unknown
    }

    public enum CharacterType
    {
        Letter,
        Digit,
        MathSymbol,
        LeftBracket,
        RightBracket,
        Separator,
        Unknown,
        Whitespace,
        Empty
    }
}
