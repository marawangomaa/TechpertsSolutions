using Core.DTOs.ServiceUsageDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Enums;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceUsageController : ControllerBase
    {
        private readonly IServiceUsageService _service;

        public ServiceUsageController(IServiceUsageService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceUsageCreateDTO dto, [FromForm] ServiceType serviceType)
        {
            // Create a new DTO with the serviceType from query parameter
            var dtoWithServiceType = new ServiceUsageCreateDTO
            {
                ServiceType = serviceType,
                UsedOn = dto.UsedOn,
                CallCount = dto.CallCount
            };

            var result = await _service.CreateAsync(dtoWithServiceType);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ServiceUsageUpdateDTO dto, [FromForm] ServiceType? serviceType = null)
        {
            // Create a new DTO with the serviceType from query parameter if provided
            var dtoWithServiceType = new ServiceUsageUpdateDTO
            {
                ServiceType = serviceType ?? dto.ServiceType,
                UsedOn = dto.UsedOn,
                CallCount = dto.CallCount
            };

            var result = await _service.UpdateAsync(id, dtoWithServiceType);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _service.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}
