using Core.DTOs.AdminDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;
using Core.Enums;
using Core.Utilities;
using Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    //[Authorize(Roles = "Admin")]
    public class AdminUserManagementController : ControllerBase
    {
        private readonly IAdminUserManagementService _adminUserManagementService;

        public AdminUserManagementController(IAdminUserManagementService adminUserManagementService)
        {
            _adminUserManagementService = adminUserManagementService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? role = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? sortBy = "createdAt",
            [FromQuery] bool sortDesc = true)
        {
            var response = await _adminUserManagementService.GetAllUsersAsync(
                pageNumber, pageSize, search, role, isActive, sortBy, sortDesc);
            
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
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

            var response = await _adminUserManagementService.GetUserByIdAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(string id)
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

            var response = await _adminUserManagementService.DeactivateUserAsync(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("{id}/activate")]
        public async Task<IActionResult> ActivateUser(string id)
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

            var response = await _adminUserManagementService.ActivateUserAsync(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("{id}/roles")]
        public async Task<IActionResult> ChangeUserRoles(string id, [FromForm] UserRolesUpdateDTO dto)
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

            if (dto == null || dto.Roles == null || !dto.Roles.Any())
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "No roles provided.",
                    Data = null
                });
            }

            // Convert enums to string values
            var roleStrings = dto.Roles.Select(r => r.GetStringValue()).ToList();

            var response = await _adminUserManagementService.ChangeUserRolesAsync(id, roleStrings);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetUserStatistics()
        {
            var response = await _adminUserManagementService.GetUserStatisticsAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
} 
