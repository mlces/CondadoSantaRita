using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public virtual ICollection<Token> Tokens { get; set; }

        public static TokenType Access { get; private set; } = new() { TokenTypeId = 1, Name = nameof(Access) };
        public static TokenType Reset { get; private set; } = new() { TokenTypeId = 2, Name = nameof(Reset) };
    }
}
