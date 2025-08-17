using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using TechpertsSolutions.Core.Entities;
using Core.Interfaces;

namespace Service
{
    public class FileService : IFileService
    {
        private readonly IHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        private readonly long _maxFileSize = 5 * 1024 * 1024;
        private readonly UserManager<AppUser> userManager;
        private readonly IRepository<Product> productRepo;
        private readonly IRepository<Category> categoryRepo;
        private readonly IRepository<SubCategory> subCategoryRepo;

        public FileService(IHostEnvironment environment, IConfiguration configuration, UserManager<AppUser> _userManager,
            IRepository<Product> _productRepo, IRepository<Category> _categoryRepo, IRepository<SubCategory> _subCategoryRepo)
        {
            _environment = environment;
            _configuration = configuration;
            userManager = _userManager;
            productRepo = _productRepo;
            categoryRepo = _categoryRepo;
            subCategoryRepo = _subCategoryRepo;
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

            if (!File.Exists(fullPath))
                return false;

            File.Delete(fullPath);

            // === Clean up image path from related entities ===

            // Products
            var productsWithImage = await productRepo.FindAsync(p => p.ImageUrl == imagePath);
            foreach (var product in productsWithImage)
            {
                product.ImageUrl = null;
                productRepo.Update(product);
            }

            // Categories
            var categoriesWithImage = await categoryRepo.FindAsync(c => c.Image == imagePath);
            foreach (var category in categoriesWithImage)
            {
                category.Image = null;
                categoryRepo.Update(category);
            }

            // SubCategories
            var subCategoriesWithImage = await subCategoryRepo.FindAsync(sc => sc.Image == imagePath);
            foreach (var subCategory in subCategoriesWithImage)
            {
                subCategory.Image = null;
                subCategoryRepo.Update(subCategory);
            }

            // AppUsers
            var usersWithImage = userManager.Users
                .Where(u => u.ProfilePhotoUrl == imagePath)
                .ToList();
            foreach (var user in usersWithImage)
            {
                user.ProfilePhotoUrl = null;
                await userManager.UpdateAsync(user); // Identity update method
            }

            // Save all repo changes
            await productRepo.SaveChangesAsync();
            await categoryRepo.SaveChangesAsync();
            await subCategoryRepo.SaveChangesAsync();

            return true;
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
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            
            return $"{fileNameWithoutExtension}_{timestamp}_{uniqueId}{extension}";
        }
    }
} 
