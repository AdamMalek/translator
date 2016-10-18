﻿using System;
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
            W();
            return true;
        }

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
        }
        void AcceptCharacter(SymbolType expectedType)
        {
            if (_currentSymbol.Type == expectedType)
            {
                _currentSymbol = _lexer.GetNext();
            }
            else
            {
                throw new InvalidSyntaxException(_currentSymbol.Position, _currentSymbol.SymbolString, null);
            }
        }
    }
}
