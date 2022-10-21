using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public partial class TokenType
    {
        public TokenType()
        {
            Tokens = new HashSet<Token>();
        }

        public int TokenTypeId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Token> Tokens { get; set; }

        public static TokenType Access => new() { TokenTypeId = 1, Name = nameof(Access) };
        public static TokenType Reset => new() { TokenTypeId = 2, Name = nameof(Reset) };
        public static TokenType Refresh => new() { TokenTypeId = 3, Name = nameof(Refresh) };

        public static bool operator ==(TokenType obj1, TokenType obj2) => obj1.Name == obj2.Name;
        public static bool operator !=(TokenType obj1, TokenType obj2) => !(obj1 == obj2);
    }
}
