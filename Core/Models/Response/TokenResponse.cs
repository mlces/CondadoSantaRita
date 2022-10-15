using Api.Tokens;

namespace Core.Models.Response
{
    public class TokenResponse<T>
    {
        public TokenType? TokenType { get; set; }
        public string? AccessToken { get; set; }
        public DateTime? ExpiresIn { get; set; }
        public T? Data { get; set; }
    }
}
