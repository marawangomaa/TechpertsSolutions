using Microsoft.AspNetCore.Http;

namespace Core.DTOs
{
    public class ImageUploadDTO
    {
        public IFormFile? Image { get; set; }
        public string? FolderName { get; set; }
    }

    public class ImageUploadResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ImagePath { get; set; }
        public string ImageUrl { get; set; }
        public List<string> ImagePaths { get; set; }
    }

    public class UserProfilePhotoUploadDTO
    {
        public IFormFile? ProfilePhoto { get; set; }
    }
} 
