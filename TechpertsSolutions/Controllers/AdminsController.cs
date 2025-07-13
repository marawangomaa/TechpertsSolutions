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
            var admin  = await adminService.GetByIdAsync(id);
            if(admin == null) return NotFound( new GeneralResponse<string> 
            {
                Success = false,
                Message = "Failed to get data",
                Data = "Admin does not exist"
            });
            return Ok(new GeneralResponse<AdminReadDTO> 
            {
                Success = true,
                Message = "retrieved admin successfully",
                Data = admin
            });
        }

        [HttpPost(nameof(Create))]
        public async Task<IActionResult> Create([FromBody] AdminCreateDTO dto) 
        {
            var created = await adminService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new {id = created.Id},created);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateRole(string id, AdminUpdateDTO dto)
        {
            var result = await adminService.UpdateRoleAsync(id,dto);
            if (!result) return NotFound(
                new GeneralResponse<string> 
                {
                    Success = false,
                    Message = "Failed to update",
                    Data = "Not Found"
                });
            return Ok(new GeneralResponse<AdminUpdateDTO> 
            {
                Success = true,
                Message = "Updated Successfully",
                Data = result
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await adminService.DeleteAsync(id);
            if (!success) return NotFound(
                new GeneralResponse<string> 
                {
                    Success = false,
                    Message = "Does not Exist",
                    Data = "Failed to delete"
                });
            return Ok(new GeneralResponse<string>{
                    Success = true,
                    Message = "Found and deleted",
                    Data = "deleted successfully"});
        }

    }
}
