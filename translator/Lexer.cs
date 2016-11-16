using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using translator.Models;

namespace translator
{
    public class Lexer
    {
        List<Symbol> symbols = new List<Symbol>();
        public List<Symbol> Symbols
        {
            get
            {
                return symbols.OrderBy(x => x.Position).ToList();
            }
        }
        Symbol current;
        public Symbol GetNext()
        {
            if (current != null)
            {
                var index = Symbols.IndexOf(current) + 1;
                if (index == Symbols.Count)
                {
                    return new Symbol { Type = SymbolType.EOF, Position= index, SymbolString="EOF"};
                }
                else
                {
                    current = Symbols.ElementAt(index);
                    return current;
                }
            }
            else
            {
                current = Symbols.First();
                return current;
            }
        }



        public Symbol[] AnalizeSource(string source, bool distinct = false)
        {
            symbols.Clear();
            this.source = source.Replace("\r", "");
            analyzeSegment(source, distinct);
            return Symbols.ToArray();
        }
        private void analyzeSegment(string segment, bool distinct)
        {
            segment = segment.Replace("\r", "");
            var symbol = ""; var symbolType = SymbolType.Empty;
            var pastChar = ""; var pastCharType = CharacterType.Empty;
            for (int i = 0; i < segment.Length; i++)
            {
                var currentChar = segment[i].ToString();
                var currentType = getSymbolType(currentChar);

                switch (currentType)
                {
                    case CharacterType.RightBracket:
                        if (pastCharType == CharacterType.Separator) throw new InvalidSyntaxException(i, currentChar, Symbols.ToArray(),SymbolType.Number);
                        if (symbolType != SymbolType.Empty)
                            AddSymbol(symbol, i - symbol.Length, symbolType, distinct);
                        AddSymbol(")", i, SymbolType.RightBracket, distinct);
                        pastChar = ""; pastCharType = CharacterType.Empty;
                        symbol = ""; symbolType = SymbolType.Empty;
                        break;
                    case CharacterType.LeftBracket:
                        if (pastCharType == CharacterType.Separator) throw new InvalidSyntaxException(i, currentChar, Symbols.ToArray(), SymbolType.Number);
                        if (symbolType != SymbolType.Empty)
                            AddSymbol(symbol, i - symbol.Length, symbolType, distinct);
                        AddSymbol("(", i, SymbolType.LeftBracket, distinct);
                        pastChar = ""; pastCharType = CharacterType.Empty;
                        symbol = ""; symbolType = SymbolType.Empty;
                        break;
                    case CharacterType.Whitespace:
                        if (pastCharType == CharacterType.Separator) throw new InvalidSyntaxException(i, currentChar, Symbols.ToArray(), SymbolType.Number);
                        if (symbolType != SymbolType.Empty)
                            AddSymbol(symbol, i - symbol.Length, symbolType, distinct);
                        pastChar = ""; pastCharType = CharacterType.Empty;
                        symbol = ""; symbolType = SymbolType.Empty;
                        break;
                    case CharacterType.MathSymbol:
                        if (pastCharType == CharacterType.Separator) throw new InvalidSyntaxException(i, currentChar, Symbols.ToArray(), SymbolType.Number);
                        if (symbolType != SymbolType.Empty)
                            AddSymbol(symbol, i - symbol.Length, symbolType, distinct);
                        AddSymbol(currentChar, i,(currentChar=="+"||currentChar=="-") ? SymbolType.AddSubtract : SymbolType.MultiplyDivide, distinct);
                        pastChar = ""; pastCharType = CharacterType.Empty;
                        symbol = ""; symbolType = SymbolType.Empty;
                        break;

                    case CharacterType.Letter:
                        if (symbolType == SymbolType.Empty || symbolType == SymbolType.Variable)
                        {
                            symbol += currentChar; symbolType = SymbolType.Variable;
                            pastChar = currentChar; pastCharType = currentType;
                        }
                        else if (symbolType == SymbolType.Integer || symbolType == SymbolType.Float)
                        {
                            if (pastCharType == CharacterType.Separator)
                                throw new InvalidSyntaxException(i, currentChar, Symbols.ToArray(), SymbolType.Number);
                            AddSymbol(symbol, i - symbol.Length, symbolType, distinct);
                            symbol = currentChar; symbolType = SymbolType.Variable;
                            pastChar = currentChar; pastCharType = currentType;
                        }
                        break;
                    case CharacterType.Digit:
                        if (symbolType == SymbolType.Empty)
                        {
                            symbol += currentChar;
                            symbolType = SymbolType.Integer;
                            pastCharType = currentType;
                            pastChar = currentChar;
                        }
                        else if (symbolType == SymbolType.Variable || symbolType == SymbolType.Integer || symbolType == SymbolType.Float)
                        {
                            symbol += currentChar;
                            pastChar = currentChar; pastCharType = currentType;
                        }
                        else throw new InvalidSyntaxException(i, currentChar, Symbols.ToArray());
                        break;
                    case CharacterType.Separator:
                        if (symbolType == SymbolType.Integer)
                        {
                            symbol += currentChar; symbolType = SymbolType.Float;
                            pastChar = currentChar; pastCharType = currentType;
                        }
                        else if (symbolType == SymbolType.Variable)
                        {
                            AddSymbol(symbol, i - symbol.Length, symbolType, distinct);
                            throw new InvalidSyntaxException(i, currentChar, Symbols.ToArray());
                        }
                        else
                        {
                            throw new InvalidSyntaxException(i, currentChar, Symbols.ToArray());
                        }
                        break;
                    default:
                        throw new InvalidSyntaxException(i, currentChar, Symbols.ToArray());
                }
            }
            if (symbolType != SymbolType.Empty)
                AddSymbol(symbol, segment.Length - symbol.Length, symbolType, distinct);
        }

        Regex letter = new Regex("[a-zA-Z]");
        Regex digit = new Regex(@"\d");
        Regex mathSymbol = new Regex(@"[+\-*/]");
        Regex separator = new Regex(@"[.]");
        private string source;

        private CharacterType getSymbolType(string character)
        {
            if (letter.IsMatch(character)) return CharacterType.Letter;
            if (digit.IsMatch(character)) return CharacterType.Digit;
            if (mathSymbol.IsMatch(character)) return CharacterType.MathSymbol;
            if (separator.IsMatch(character)) return CharacterType.Separator;
            switch (character)
            {
                case "(":
                    return CharacterType.LeftBracket;
                case ")":
                    return CharacterType.RightBracket;
                case " ":
                    return CharacterType.Whitespace;
                case "\n":
                    return CharacterType.Whitespace;
                case "\t":
                    return CharacterType.Whitespace;
                default: return CharacterType.Unknown;
            }
        }
        public void AddSymbol(string symbol, int position, SymbolType type, bool distinct)
        {
            if (distinct)
            {
                if (!symbols.Select(x => x.SymbolString).Contains(symbol))
                {
                    symbols.Add(new Symbol { SymbolString = symbol, Position = position, Type = type });
                }
            }
            else
            {
                symbols.Add(new Symbol { SymbolString = symbol, Position = position, Type = type });
            }
        }
    }
}
