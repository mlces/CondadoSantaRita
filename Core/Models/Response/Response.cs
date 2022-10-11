namespace Core.Models.Response
{
    public class Response<T>
    {
        public int Code { get; set; } = -1;
        public string? Message { get; set; } = null;
        public T? Data { get; set; } = default;
    }
}
