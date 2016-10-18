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
        public int Position { get; set; }
        public Symbol[] Symbols { get; set; }
        public override string Message
        {
            get
            {
                return $"Error in position: { Position }, near character {character}";
            }
        }
        public InvalidSyntaxException(int pos, string character, Symbol[] symbols)
        {
            Symbols = symbols;
            Position = pos;
            this.character = character;
        }
    }
}
