using Microsoft.AspNetCore.Mvc;
using Core.Interfaces.Services;
using Core.DTOs.SubCategoryDTOs;
using TechpertsSolutions.Core.DTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    
    
    
    [ApiController]
    [Route("api/[controller]")]
    
    public class SubCategoryController : ControllerBase
    {
        private readonly ISubCategoryService _subCategoryService;
        private readonly ILogger<SubCategoryController> _logger;

        public SubCategoryController(ISubCategoryService subCategoryService, ILogger<SubCategoryController> logger)
        {
            _subCategoryService = subCategoryService;
            _logger = logger;
        }

        
        
        
        
        [HttpGet]
        [ProducesResponseType(typeof(GeneralResponse<IEnumerable<SubCategoryDTO>>), 200)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 500)]
        public async Task<IActionResult> GetAllSubCategories()
        {
            try
            {
                var response = await _subCategoryService.GetAllSubCategoriesAsync();
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all subcategories.");
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while retrieving subcategories."
                });
            }
        }

        
        
        
        
        
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GeneralResponse<SubCategoryDTO>), 200)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 404)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 500)]
        public async Task<IActionResult> GetSubCategoryById(string id)
        {
            try
            {
                var response = await _subCategoryService.GetSubCategoryByIdAsync(id);
                if (!response.Success)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting subcategory by ID: {id}");
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the subcategory."
                });
            }
        }

        
        
        
        
        
        [HttpGet("byCategory/{categoryId}")]
        [ProducesResponseType(typeof(GeneralResponse<IEnumerable<SubCategoryDTO>>), 200)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 400)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 500)]
        public async Task<IActionResult> GetSubCategoriesByCategoryId(string categoryId)
        {
            try
            {
                var response = await _subCategoryService.GetSubCategoriesByCategoryIdAsync(categoryId);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting subcategories by Category ID: {categoryId}");
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while retrieving subcategories by category."
                });
            }
        }

        
        
        
        
        
        [HttpPost]
        
        [ProducesResponseType(typeof(GeneralResponse<SubCategoryDTO>), 201)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 400)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 500)]
        public async Task<IActionResult> CreateSubCategory([FromBody] CreateSubCategoryDTO createDto)
        {
            if (!ModelState.IsValid)
            {
                
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Validation failed: " + string.Join("; ", errors),
                    Data = "Invalid form data"
                });
            }

            try
            {
                var response = await _subCategoryService.CreateSubCategoryAsync(createDto);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return CreatedAtAction(
                    nameof(GetSubCategoryById),
                    new { id = response.Data?.Id },
                    response
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subcategory.");
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while creating the subcategory.",
                    Data = ex.Message
                });
            }
        }

        
        
        
        
        
        
        [HttpPut("{id}")]
        
        [ProducesResponseType(typeof(GeneralResponse<string>), 200)] 
        [ProducesResponseType(typeof(GeneralResponse<string>), 400)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 404)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 500)]
        public async Task<IActionResult> UpdateSubCategory(string id, [FromBody] UpdateSubCategoryDTO updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "ID in URL does not match ID in body.",
                    Data = "Route ID: " + id + ", Body ID: " + updateDto.Id
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Validation failed: " + string.Join("; ", errors),
                    Data = "Invalid form data"
                });
            }

            try
            {
                var response = await _subCategoryService.UpdateSubCategoryAsync(updateDto);
                if (!response.Success)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating subcategory with ID: {id}");
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while updating the subcategory.",
                    Data = ex.Message
                });
            }
        }

        
        
        
        
        
        [HttpDelete("{id}")]
        
        [ProducesResponseType(typeof(GeneralResponse<string>), 200)] 
        [ProducesResponseType(typeof(GeneralResponse<string>), 404)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 500)]
        public async Task<IActionResult> DeleteSubCategory(string id)
        {
            try
            {
                var response = await _subCategoryService.DeleteSubCategoryAsync(id);
                if (!response.Success)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting subcategory with ID: {id}");
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while deleting the subcategory."
                });
            }
        }

        
        
        
        
        
        
        [HttpPost("{subCategoryId}/upload-image")]
        
        [ProducesResponseType(typeof(GeneralResponse<ImageUploadResponseDTO>), 200)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 400)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 500)]
        public async Task<IActionResult> UploadSubCategoryImage(string subCategoryId, IFormFile imageFile)
        {
            if (string.IsNullOrWhiteSpace(subCategoryId) || !Guid.TryParse(subCategoryId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid subcategory ID.",
                    Data = subCategoryId
                });
            }

            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "No image file provided.",
                    Data = null
                });
            }

            try
            {
                var response = await _subCategoryService.UploadSubCategoryImageAsync(imageFile, subCategoryId);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading image for subcategory with ID: {subCategoryId}");
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while uploading the image.",
                    Data = ex.Message
                });
            }
        }

        
        
        
        
        
        [HttpDelete("{subCategoryId}/delete-image")]
        
        [ProducesResponseType(typeof(GeneralResponse<bool>), 200)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 400)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 500)]
        public async Task<IActionResult> DeleteSubCategoryImage(string subCategoryId)
        {
            if (string.IsNullOrWhiteSpace(subCategoryId) || !Guid.TryParse(subCategoryId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid subcategory ID.",
                    Data = subCategoryId
                });
            }

            try
            {
                var response = await _subCategoryService.DeleteSubCategoryImageAsync(subCategoryId);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting image for subcategory with ID: {subCategoryId}");
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while deleting the image.",
                    Data = ex.Message
                });
            }
        }
    }
}
