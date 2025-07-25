using Core.DTOs.SalesManagerDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesManagerController : ControllerBase
    {
        private readonly ISalesManagerService _salesManagerService;

        public SalesManagerController(ISalesManagerService salesManagerService)
        {
            _salesManagerService = salesManagerService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] SalesManagerCreateDTO dto)
        {
            var result = await _salesManagerService.CreateAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _salesManagerService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _salesManagerService.GetAllAsync();
            return Ok(result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SalesManagerUpdateDTO dto)
        {
            var result = await _salesManagerService.UpdateAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _salesManagerService.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}
