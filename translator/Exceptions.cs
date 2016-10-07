using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace translator
{
    public class InvalidSyntaxException : ApplicationException
    {
        private string character;
        public int Position { get; set; }
        public override string Message
        {
            get
            {
                return $"Error in position: { Position }, near character {character}";
            }
        }
        public InvalidSyntaxException(int pos, string character)
        {
            Position = pos;
            this.character = character;
        }
    }
}
