namespace Core.Models.Response
{
    public class TokenResponse<T>
    {
        public string? AccessToken { get; set; }

        public DateTime? ExpiresIn { get; set; }

        public T? Data { get; set; }
    }
}
