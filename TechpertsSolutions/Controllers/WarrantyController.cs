using Core.DTOs.WarrantyDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarrantyController : ControllerBase
    {
        private readonly IWarrantyService _warrantyService;

        public WarrantyController(IWarrantyService warrantyService)
        {
            _warrantyService = warrantyService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WarrantyCreateDTO dto)
        {
            var result = await _warrantyService.CreateAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _warrantyService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _warrantyService.GetAllAsync();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] WarrantyUpdateDTO dto)
        {
            var result = await _warrantyService.UpdateAsync(id, dto);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _warrantyService.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}
