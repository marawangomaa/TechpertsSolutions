using Core.DTOs.Admin;
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
            var admins = await _adminService.GetAllAsync();
            return Ok(new GeneralResponse<List<AdminReadDTO>>
            {
                Success = true,
                Message = "Retrieved admin list successfully",
                Data = admins.ToList()
            });
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

            try
            {
                var admin = await _adminService.GetByIdAsync(id);

                if (admin == null)
                    return NotFound(new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "Admin not found.",
                        Data = $"No admin with ID {id}."
                    });

                return Ok(new GeneralResponse<AdminReadDTO>
                {
                    Success = true,
                    Message = "Admin retrieved successfully.",
                    Data = admin
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An unexpected error occurred.",
                    Data = ex.Message
                });
            }
        }
    }
}
