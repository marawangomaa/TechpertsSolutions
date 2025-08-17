using Core.DTOs;
using Core.DTOs.CustomerDTOs;
using Core.DTOs.ProfileDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechpertsSolutions.Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _env;

        public UserManagementController(
            IUserManagementService userManagementService,
            IFileService fileService,
            IWebHostEnvironment env)
        {
            _userManagementService = userManagementService;
            _fileService = fileService;
            _env = env;
        }

        [HttpPut("{id}/profile")]
        public async Task<IActionResult> UpdateProfile(string id, [FromForm] UserProfileUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid user ID.",
                    Data = id
                });
            }

            var response = await _userManagementService.UpdateProfileAsync(id,dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("{id}/upload-profile-photo")]
        public async Task<IActionResult> UploadProfilePhoto(string id, IFormFile photoFile)
        {
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid user ID.",
                    Data = id
                });
            }

            if (photoFile == null || photoFile.Length == 0)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "No photo file provided."
                });
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(photoFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid file type. Only JPG, JPEG, PNG, and GIF files are allowed."
                });
            }

            // Validate file size (max 5MB)
            if (photoFile.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "File size too large. Maximum size is 5MB."
                });
            }

            var response = await _userManagementService.UploadProfilePhotoAsync(id, photoFile);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("{id}/services-usage")]
        public async Task<IActionResult> GetUserServicesUsage(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid user ID.",
                    Data = id
                });
            }

            var response = await _userManagementService.GetUserServicesUsageAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
} 
