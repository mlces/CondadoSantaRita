namespace Core.Models.Response
{
    public class Response<T>
    {
        public ResponseCode Code { get; set; } = ResponseCode.BadRequest;
        public string? Message { get; set; } = null;
        public T? Data { get; set; } = default;
    }
}
