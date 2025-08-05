using Core.DTOs;
using Core.DTOs.CustomerDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TechpertsSolutions.Controllers
{
    [ApiController]
    [Route("api/user/profile")]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IFileService _fileService;

        public UserProfileController(IUserManagementService userManagementService, IFileService fileService)
        {
            _userManagementService = userManagementService;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "User not authenticated.",
                    Data = null
                });
            }

            var response = await _userManagementService.GetUserProfileAsync(userId);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserProfile([FromForm] UserProfileUpdateDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "User not authenticated.",
                    Data = null
                });
            }

            var response = await _userManagementService.UpdateUserProfileAsync(userId, dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("upload-photo")]
        public async Task<IActionResult> UploadProfilePhoto([FromForm] UserProfilePhotoUploadDTO dto)
        {
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "User not authenticated.",
                    Data = null
                });
            }

            if (dto.ProfilePhoto == null || dto.ProfilePhoto.Length == 0)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Profile photo is required.",
                    Data = null
                });
            }

            var response = await _userManagementService.UploadProfilePhotoAsync(userId, dto.ProfilePhoto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
} 