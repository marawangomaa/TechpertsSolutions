using Core.DTOs.PCAssemblyDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;
using Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    [ApiController]
    [Route("api/assembly")]
    //[Authorize]
    public class PCAssemblyCompatibilityController : ControllerBase
    {
        private readonly IPCAssemblyCompatibilityService _pcAssemblyCompatibilityService;

        public PCAssemblyCompatibilityController(IPCAssemblyCompatibilityService pcAssemblyCompatibilityService)
        {
            _pcAssemblyCompatibilityService = pcAssemblyCompatibilityService;
        }

        [HttpGet("compatible")]
        public async Task<IActionResult> GetCompatibleParts(
            [FromQuery] string? categoryId = null,
            [FromQuery] string? subCategoryId = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? techCompanyId = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var filter = new CompatibilityFilterDTO
            {
                CategoryId = categoryId,
                SubCategoryId = subCategoryId,
                MaxPrice = maxPrice,
                TechCompanyId = techCompanyId
            };

            var response = await _pcAssemblyCompatibilityService.GetCompatiblePartsAsync(
                filter, pageNumber, pageSize);
            
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetComponentsByFilter(
            [FromQuery] string? componentId = null,
            [FromQuery] string? categoryId = null,
            [FromQuery] string? subCategoryId = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? techCompanyId = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var filter = new CompatibilityFilterDTO
            {
                ComponentId = componentId,
                CategoryId = categoryId,
                SubCategoryId = subCategoryId,
                MaxPrice = maxPrice,
                TechCompanyId = techCompanyId
            };

            var response = await _pcAssemblyCompatibilityService.GetComponentsByFilterAsync(
                filter, pageNumber, pageSize);
            
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("categories/{categoryId}/compatible")]
        public async Task<IActionResult> GetCompatiblePartsByCategory(
            string categoryId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(categoryId) || !Guid.TryParse(categoryId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid category ID.",
                    Data = categoryId
                });
            }

            var response = await _pcAssemblyCompatibilityService.GetCompatiblePartsByCategoryAsync(
                categoryId, pageNumber, pageSize);
            
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("check-compatibility")]
        public async Task<IActionResult> CheckCompatibility(
            [FromQuery] string productId1,
            [FromQuery] string productId2)
        {
            if (string.IsNullOrWhiteSpace(productId1) || !Guid.TryParse(productId1, out _) ||
                string.IsNullOrWhiteSpace(productId2) || !Guid.TryParse(productId2, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid product IDs."
                });
            }

            var response = await _pcAssemblyCompatibilityService.CheckCompatibilityAsync(productId1, productId2);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
} 
