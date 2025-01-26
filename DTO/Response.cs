namespace MediMitra.DTO
{
    public class Response<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; } = String.Empty;
        public string Type { get; set; } = String.Empty;
        public T Data { get; set; }
    }
}
