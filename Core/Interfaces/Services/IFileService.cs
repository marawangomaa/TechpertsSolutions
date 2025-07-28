using Microsoft.AspNetCore.Http;

namespace Core.Interfaces.Services
{
    public interface IFileService
    {
        Task<string> UploadImageAsync(IFormFile file, string controllerName);
        Task<bool> DeleteImageAsync(string imagePath);
        Task<string> UpdateImageAsync(IFormFile newFile, string oldImagePath, string controllerName);
        bool IsValidImageFile(IFormFile file);
        string GetImageUrl(string imagePath);
    }
} 