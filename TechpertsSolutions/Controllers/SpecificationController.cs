using Core.DTOs.Specifications;
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
            var specs = await _specificationService.GetAllSpecificationsAsync();
            return Ok(new GeneralResponse<IEnumerable<SpecificationDTO>> 
            {
                Success = true,
                Message = "Successfully",
                Data = specs
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var spec = await _specificationService.GetSpecificationByIdAsync(id);
            if (spec == null)
                return NotFound(new GeneralResponse<string> { Success = false, Message = "Specification not found." });

            return Ok(new GeneralResponse<SpecificationDTO> 
            {
                Success = true,
                Message = $"Specifying id {id}",
                Data = spec
            });
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductId(string productId)
        {
            var specs = await _specificationService.GetSpecificationsByProductIdAsync(productId);
            if (!specs.Any())
                return NotFound(new GeneralResponse<string> { Success = false, Message = "No specifications found for this product." });

            return Ok(new GeneralResponse<IEnumerable<SpecificationDTO>>
            {
                Success = true,
                Message = "Successfully",
                Data = specs
            });
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse<SpecificationDTO>>> Create([FromBody] CreateSpecificationDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse<string> { Success = false, Message = "Invalid data." });

            var created = await _specificationService.CreateSpecificationAsync(dto);
            if (created == null)
                return BadRequest(new GeneralResponse<string> { Success = false, Message = "Invalid Product ID." });

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, new GeneralResponse<SpecificationDTO>
            {
                Success = true,
                Message = "Specification created successfully.",
                Data = created
            });
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] UpdateSpecificationDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse<string> { Success = false, Message = "Invalid data." });

            var updated = await _specificationService.UpdateSpecificationAsync(dto);
            if (!updated)
                return NotFound(new GeneralResponse<string> { Success = false, Message = "Specification or Product not found." });

            return Ok(new GeneralResponse<string>
            {
                Success = true,
                Message = $"Updated successfully",
                Data = "Done"
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var deleted = await _specificationService.DeleteSpecificationAsync(id);
            if (!deleted)
                return NotFound(new GeneralResponse<string> { Success = false, Message = "Specification not found." });

            return Ok(new GeneralResponse<string> 
            {
                Success = true,
                Message = $"deleted successfully",
                Data = "Done"
            });
        }
    }
}