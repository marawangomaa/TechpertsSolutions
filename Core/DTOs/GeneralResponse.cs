namespace TechpertsSolutions.Core.DTOs
{
    public class GeneralResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }
    }
}
