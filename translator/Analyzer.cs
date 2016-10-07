using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using translator.Models;

namespace translator
{
    public class Analyzer
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
        char[] mathSigns = new char[] { '+', '-', '*', '/' };
        public Symbol GetNext()
        {
            current = current == null ? current : Symbols.First();
            var index = Symbols.IndexOf(current) + 1;
            if (index == Symbols.Count)
            {
                return null;
            }
            else
            {
                current = Symbols.ElementAt(index);
                return current;
            }
        }

        public string[] AnalizeSource(string source, bool distinct = true)
        {
            //for (int i = 0; i < source.Length; i++)
            //{
            //    if (mathSymbol.IsMatch(source[i].ToString()))
            //    {
            //        AddSymbol(source[i].ToString(), i, SymbolType.MathSymbol, distinct);
            //    }
            //}
            //string src = processWhiteSpaceInput(source, distinct);
            //var segments = src.Split(mathSigns);
            //if (segments.Contains(""))
            //{
            //    throw new InvalidSyntaxException();
            //}
            //foreach (var segment in segments)
            symbols.Clear();
            this.source = source.Replace("\r","");
            {
                analyzeSegment(source, distinct);
            }
            return Symbols.Select(x => x.Displayname).ToArray();
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
                        if (pastCharType == CharacterType.Separator) throw new InvalidSyntaxException(i, currentChar);
                        if (symbolType != SymbolType.Empty)
                            AddSymbol(symbol, i - symbol.Length, symbolType, distinct);
                        AddSymbol(")", i, SymbolType.RightBracket, distinct);
                        pastChar = ""; pastCharType = CharacterType.Empty;
                        symbol = ""; symbolType = SymbolType.Empty;
                        break;
                    case CharacterType.LeftBracket:
                        if (pastCharType == CharacterType.Separator) throw new InvalidSyntaxException(i, currentChar);
                        if (symbolType != SymbolType.Empty)
                            AddSymbol(symbol, i - symbol.Length, symbolType, distinct);
                        AddSymbol("(", i, SymbolType.LeftBracket, distinct);
                        pastChar = ""; pastCharType = CharacterType.Empty;
                        symbol = ""; symbolType = SymbolType.Empty;
                        break;
                    case CharacterType.Whitespace:
                        if (pastCharType == CharacterType.Separator) throw new InvalidSyntaxException(i, currentChar);
                        if (symbolType != SymbolType.Empty)
                            AddSymbol(symbol, i - symbol.Length, symbolType, distinct);
                        pastChar = ""; pastCharType = CharacterType.Empty;
                        symbol = ""; symbolType = SymbolType.Empty;
                        break;
                    case CharacterType.MathSymbol:
                        if (pastCharType == CharacterType.Separator) throw new InvalidSyntaxException(i, currentChar);
                        if (symbolType != SymbolType.Empty)
                            AddSymbol(symbol, i - symbol.Length, symbolType, distinct);
                        AddSymbol(currentChar, i, SymbolType.MathSymbol, distinct);
                        pastChar = ""; pastCharType = CharacterType.Empty;
                        symbol = ""; symbolType = SymbolType.Empty;
                        break;

                    case CharacterType.Letter:
                        if (symbolType == SymbolType.Empty || symbolType == SymbolType.Variable)
                        {
                            symbol += currentChar; symbolType = SymbolType.Variable;
                            pastChar = currentChar; pastCharType = currentType;
                        }
                        else
                        {
                            throw new InvalidSyntaxException(i, currentChar);
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
                        else throw new InvalidSyntaxException(i, currentChar);
                        break;
                    case CharacterType.Separator:
                        if (symbolType == SymbolType.Integer)
                        {
                            symbol += currentChar; symbolType = SymbolType.Float;
                            pastChar = currentChar; pastCharType = currentType;
                        }
                        else
                        {
                            throw new InvalidSyntaxException(i, currentChar);
                        }
                        break;
                    default:
                        throw new InvalidSyntaxException(i, currentChar);
                }
            }
            if (symbolType != SymbolType.Empty)
                AddSymbol(symbol, segment.Length-symbol.Length, symbolType, distinct);
        }

        Regex letter = new Regex("[a-zA-Z]");
        Regex digit = new Regex(@"\d");
        Regex mathSymbol = new Regex(@"[+\-*/]");
        Regex separator = new Regex(@"[,.]");
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
        private string processWhiteSpaceInput(string src, bool distinct)
        {
            var tabulators = src.IndexOf("\t");
            if (tabulators > -1)
            {
                //if (distinct)
                //{
                AddSymbol("\t", tabulators, SymbolType.Whitespace, distinct);
                //}
                //else
                //{
                //    foreach (var tab in src.Where()
                //    {

                //    }
                //}
                src = src.Replace("\t", " ");
            }

            var space = src.IndexOf(" ");
            if (space > -1)
            {
                AddSymbol(" ", space, SymbolType.Whitespace, distinct);
                //src = src.Replace("  ", " ");
            }
            var newLine = src.IndexOf("\r\n");
            if (newLine > -1)
            {
                AddSymbol("\r\n", newLine, SymbolType.Whitespace, distinct);
                src = src.Replace("\r\n", " ");
            }
            //var sss = src.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //for (int i = 0; i < sss.Length - 1; i++)
            //{
            //    var last = sss[i].Last();
            //    var first = sss[i + 1].First();
            //    if (!mathSigns.Contains(last) &&
            //         !mathSigns.Contains(first))
            //        throw new InvalidSyntaxException();
            //}
            //src = src.Replace(" ", "");
            return src;
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
