using Core.DTOs.AdminDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminsController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _adminService.GetAllAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "ID must not be null or empty.",
                    Data = "Invalid input"
                });

            if (!Guid.TryParse(id, out _))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid ID format. Must be a valid GUID.",
                    Data = "Invalid GUID format"
                });

            var response = await _adminService.GetByIdAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
