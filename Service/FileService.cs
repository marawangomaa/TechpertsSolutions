using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace Service
{
    public class FileService : IFileService
    {
        private readonly IHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        private readonly long _maxFileSize = 5 * 1024 * 1024; 

        public FileService(IHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string controllerName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file provided");

            if (!IsValidImageFile(file))
                throw new ArgumentException("Invalid image file");

            
            var assetsPath = Path.Combine(_environment.ContentRootPath, "wwwroot", "assets");
            var controllerPath = Path.Combine(assetsPath, controllerName.ToLower());
            
            if (!Directory.Exists(assetsPath))
                Directory.CreateDirectory(assetsPath);
            
            if (!Directory.Exists(controllerPath))
                Directory.CreateDirectory(controllerPath);

            
            var fileName = GenerateUniqueFileName(file.FileName);
            var filePath = Path.Combine(controllerPath, fileName);

            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            
            return Path.Combine("assets", controllerName.ToLower(), fileName).Replace("\\", "/");
        }

        public async Task<bool> DeleteImageAsync(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return false;

            var fullPath = Path.Combine(_environment.ContentRootPath, "wwwroot", imagePath);
            
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }

            return false;
        }

        public async Task<string> UpdateImageAsync(IFormFile newFile, string oldImagePath, string controllerName)
        {
            
            if (!string.IsNullOrEmpty(oldImagePath))
            {
                await DeleteImageAsync(oldImagePath);
            }

            
            return await UploadImageAsync(newFile, controllerName);
        }

        public bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            
            if (file.Length > _maxFileSize)
                return false;

            
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                return false;

            
            if (!file.ContentType.StartsWith("image/"))
                return false;

            return true;
        }

        public string GetImageUrl(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return string.Empty;

            var baseUrl = _configuration["BaseUrl"] ?? "https://localhost:5001";
            return $"{baseUrl}/{imagePath.Replace("\\", "/")}";
        }

        private string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
            
            
            fileNameWithoutExtension = Regex.Replace(fileNameWithoutExtension, @"[^a-zA-Z0-9]", "");
            
            
            var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            
            return $"{fileNameWithoutExtension}_{timestamp}_{uniqueId}{extension}";
        }
    }
} 
