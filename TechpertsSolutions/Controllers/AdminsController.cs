using Core.DTOs.Admin;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService adminService;
        public AdminsController(IAdminService _adminService) 
        {
            adminService = _adminService;
        }
        [HttpGet("All")]
        public async Task<IActionResult> GetAll() 
        {
            var admins = await adminService.GetAllAsync();
            return Ok(new GeneralResponse<List<AdminReadDTO>>
            {
                Success = true,
                Message = "retrieved admin list",
                Data = admins
            });
        }
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(string id) 
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "ID must not be null or empty",
                    Data = "Invalid input"
                });

            if (!Guid.TryParse(id, out Guid guidId))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "ID format is invalid",
                    Data = "Expected GUID"
                });

            try
            {
                var admin = await adminService.GetByIdAsync(id);
                return Ok(new GeneralResponse<AdminReadDTO>
                {
                    Success = true,
                    Message = "Retrieved admin successfully",
                    Data = admin
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Admin not found",
                    Data = ex.Message
                });
            }
        }

    }
}
