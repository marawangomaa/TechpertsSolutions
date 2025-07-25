using Core.DTOs.PCAssemblyDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PCAssemblyController : ControllerBase
    {
        private readonly IPCAssemblyService _pcAssemblyService;

        public PCAssemblyController(IPCAssemblyService pcAssemblyService)
        {
            _pcAssemblyService = pcAssemblyService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PCAssemblyCreateDTO dto)
        {
            var result = await _pcAssemblyService.CreateAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _pcAssemblyService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _pcAssemblyService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(string customerId)
        {
            var result = await _pcAssemblyService.GetByCustomerIdAsync(customerId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PCAssemblyUpdateDTO dto)
        {
            var result = await _pcAssemblyService.UpdatePCAssemblyAsync(id, dto);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}
