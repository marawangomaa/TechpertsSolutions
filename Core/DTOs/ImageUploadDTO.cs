namespace Core.DTOs
{
    public class ImageUploadResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ImagePath { get; set; }
        public string ImageUrl { get; set; }
    }
} 
