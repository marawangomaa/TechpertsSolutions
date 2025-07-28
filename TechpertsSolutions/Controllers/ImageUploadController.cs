using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace TechpertsSolutions.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageUploadController : ControllerBase
    {
        private readonly IFileService _fileService;

        public ImageUploadController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload/{controllerName}")]
        public async Task<IActionResult> UploadImage(string controllerName, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest(new ImageUploadResponseDTO
                {
                    Success = false,
                    Message = "No image file provided"
                });
            }

            if (!_fileService.IsValidImageFile(imageFile))
            {
                return BadRequest(new ImageUploadResponseDTO
                {
                    Success = false,
                    Message = "Invalid image file. Please upload a valid image (jpg, jpeg, png, gif, bmp, webp) with size less than 5MB"
                });
            }

            try
            {
                var imagePath = await _fileService.UploadImageAsync(imageFile, controllerName);
                var imageUrl = _fileService.GetImageUrl(imagePath);

                return Ok(new ImageUploadResponseDTO
                {
                    Success = true,
                    Message = "Image uploaded successfully",
                    ImagePath = imagePath,
                    ImageUrl = imageUrl
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ImageUploadResponseDTO
                {
                    Success = false,
                    Message = $"Error uploading image: {ex.Message}"
                });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteImage([FromQuery] string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return BadRequest(new { Success = false, Message = "Image path is required" });
            }

            try
            {
                var deleted = await _fileService.DeleteImageAsync(imagePath);
                if (deleted)
                {
                    return Ok(new { Success = true, Message = "Image deleted successfully" });
                }
                else
                {
                    return NotFound(new { Success = false, Message = "Image not found" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = $"Error deleting image: {ex.Message}" });
            }
        }

        [HttpGet("validate")]
        public IActionResult ValidateImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest(new { Success = false, Message = "No image file provided" });
            }

            var isValid = _fileService.IsValidImageFile(imageFile);
            return Ok(new { Success = isValid, Message = isValid ? "Image is valid" : "Invalid image file" });
        }
    }
} 
