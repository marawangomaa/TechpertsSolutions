using Core.DTOs.SpecificationsDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class SpecificationController : ControllerBase
    {
        private readonly ISpecificationService _specificationService;

        public SpecificationController(ISpecificationService specificationService)
        {
            _specificationService = specificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _specificationService.GetAllSpecificationsAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _specificationService.GetSpecificationByIdAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductId(string productId)
        {
            var response = await _specificationService.GetSpecificationsByProductIdAsync(productId);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse<SpecificationDTO>>> Create([FromBody] CreateSpecificationDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new GeneralResponse<string> 
                { 
                    Success = false, 
                    Message = "Validation failed: " + string.Join("; ", errors) 
                });
            }

            var response = await _specificationService.CreateSpecificationAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return CreatedAtAction(nameof(GetById), new { id = response.Data?.Id }, response);
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] UpdateSpecificationDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new GeneralResponse<string> 
                { 
                    Success = false, 
                    Message = "Validation failed: " + string.Join("; ", errors) 
                });
            }

            var response = await _specificationService.UpdateSpecificationAsync(dto);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var response = await _specificationService.DeleteSpecificationAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}