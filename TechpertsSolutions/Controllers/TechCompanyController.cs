using Core.DTOs;
using Core.DTOs.TechCompanyDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechCompanyController : ControllerBase
    {
        private readonly ITechCompanyService _service;

        public TechCompanyController(ITechCompanyService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (IsInvalidGuid(id, out var errorResponse))
                return BadRequest(errorResponse);

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
        public async Task<IActionResult> Update(string id, [FromBody] TechCompanyUpdateDTO dto)
        {
            if (IsInvalidGuid(id, out var errorResponse))
                return BadRequest(errorResponse);

            if (dto == null)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Request body cannot be null.",
                    Data = null
                });
            }

            var result = await _service.UpdateAsync(id, dto);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // Helper to validate GUID
        private bool IsInvalidGuid(string id, out GeneralResponse<string> errorResponse)
        {
            errorResponse = null;
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
            {
                errorResponse = new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid or missing ID. Expected GUID format.",
                    Data = id
                };
                return true;
            }
            return false;
        }
    }
}