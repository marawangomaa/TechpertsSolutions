using Core.DTOs.SubCategory;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization; // For [Authorize] attribute
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs; // Assuming GeneralResponse is here


namespace TechpertsSolutions.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // Uncomment if you want to secure all endpoints in this controller
    public class SubCategoryController : ControllerBase
    {
        private readonly ISubCategoryService _subCategoryService;
        private readonly ILogger<SubCategoryController> _logger;

        public SubCategoryController(ISubCategoryService subCategoryService, ILogger<SubCategoryController> logger)
        {
            _subCategoryService = subCategoryService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all subcategories.
        /// </summary>
        /// <returns>A list of SubCategoryDto wrapped in GeneralResponse.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(GeneralResponse<IEnumerable<SubCategoryDTO>>), 200)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 500)]
        public async Task<IActionResult> GetAllSubCategories()
        {
            try
            {
                var subCategories = await _subCategoryService.GetAllSubCategoriesAsync();
                return Ok(new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = true,
                    Message = "Subcategories retrieved successfully.",
                    Data = subCategories
                });
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

        /// <summary>
        /// Gets a subcategory by its ID.
        /// </summary>
        /// <param name="id">The ID of the subcategory.</param>
        /// <returns>A SubCategoryDto wrapped in GeneralResponse if found, otherwise 404 Not Found or 500 Internal Server Error.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GeneralResponse<SubCategoryDTO>), 200)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 404)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 500)]
        public async Task<IActionResult> GetSubCategoryById(string id)
        {
            try
            {
                var subCategory = await _subCategoryService.GetSubCategoryByIdAsync(id);
                if (subCategory == null)
                {
                    return NotFound(new GeneralResponse<string>
                    {
                        Success = false,
                        Message = $"SubCategory with ID '{id}' not found."
                    });
                }
                return Ok(new GeneralResponse<SubCategoryDTO>
                {
                    Success = true,
                    Message = "SubCategory retrieved successfully.",
                    Data = subCategory
                });
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

        /// <summary>
        /// Gets subcategories by their parent Category ID.
        /// </summary>
        /// <param name="categoryId">The ID of the parent category.</param>
        /// <returns>A list of SubCategoryDto wrapped in GeneralResponse.</returns>
        [HttpGet("byCategory/{categoryId}")]
        [ProducesResponseType(typeof(GeneralResponse<IEnumerable<SubCategoryDTO>>), 200)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 400)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 500)]
        public async Task<IActionResult> GetSubCategoriesByCategoryId(string categoryId)
        {
            try
            {
                var subCategories = await _subCategoryService.GetSubCategoriesByCategoryIdAsync(categoryId);
                return Ok(new GeneralResponse<IEnumerable<SubCategoryDTO>>
                {
                    Success = true,
                    Message = $"Subcategories for Category ID '{categoryId}' retrieved successfully.",
                    Data = subCategories
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = ex.Message
                });
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

        /// <summary>
        /// Creates a new subcategory.
        /// </summary>
        /// <param name="createDto">The SubCategory data to create.</param>
        /// <returns>The created SubCategoryDto wrapped in GeneralResponse.</returns>
        [HttpPost]
        // [Authorize(Roles = "Admin,TechManager")] // Example: Only Admin or TechManager can create
        [ProducesResponseType(typeof(GeneralResponse<SubCategoryDTO>), 201)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 400)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 500)]
        public async Task<IActionResult> CreateSubCategory([FromBody] CreateSubCategoryDTO createDto)
        {
            if (!ModelState.IsValid)
            {
                // Return validation errors wrapped in GeneralResponse
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Validation failed: " + string.Join("; ", errors)
                });
            }

            try
            {
                var newSubCategory = await _subCategoryService.CreateSubCategoryAsync(createDto);
                return CreatedAtAction(
                    nameof(GetSubCategoryById),
                    new { id = newSubCategory.Id },
                    new GeneralResponse<SubCategoryDTO>
                    {
                        Success = true,
                        Message = "SubCategory created successfully.",
                        Data = newSubCategory
                    }
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subcategory.");
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while creating the subcategory."
                });
            }
        }

        /// <summary>
        /// Updates an existing subcategory.
        /// </summary>
        /// <param name="id">The ID of the subcategory to update.</param>
        /// <param name="updateDto">The updated SubCategory data.</param>
        /// <returns>200 OK with GeneralResponse if successful, 400 Bad Request, 404 Not Found, or 500 Internal Server Error.</returns>
        [HttpPut("{id}")]
        // [Authorize(Roles = "Admin,TechManager")] // Example: Only Admin or TechManager can update
        [ProducesResponseType(typeof(GeneralResponse<string>), 200)] // Changed from 204 to 200 for consistent GeneralResponse
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
                    Message = "ID in URL does not match ID in body."
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Validation failed: " + string.Join("; ", errors)
                });
            }

            try
            {
                var result = await _subCategoryService.UpdateSubCategoryAsync(updateDto);
                if (!result)
                {
                    return NotFound(new GeneralResponse<string>
                    {
                        Success = false,
                        Message = $"SubCategory with ID '{id}' not found."
                    });
                }
                return Ok(new GeneralResponse<string> // Return 200 OK with a success message
                {
                    Success = true,
                    Message = "SubCategory updated successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating subcategory with ID: {id}");
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while updating the subcategory."
                });
            }
        }

        /// <summary>
        /// Deletes a subcategory by its ID.
        /// </summary>
        /// <param name="id">The ID of the subcategory to delete.</param>
        /// <returns>200 OK with GeneralResponse if successful, 404 Not Found, or 500 Internal Server Error.</returns>
        [HttpDelete("{id}")]
        // [Authorize(Roles = "Admin,TechManager")] // Example: Only Admin or TechManager can delete
        [ProducesResponseType(typeof(GeneralResponse<string>), 200)] // Changed from 204 to 200 for consistent GeneralResponse
        [ProducesResponseType(typeof(GeneralResponse<string>), 404)]
        [ProducesResponseType(typeof(GeneralResponse<string>), 500)]
        public async Task<IActionResult> DeleteSubCategory(string id)
        {
            try
            {
                var result = await _subCategoryService.DeleteSubCategoryAsync(id);
                if (!result)
                {
                    return NotFound(new GeneralResponse<string>
                    {
                        Success = false,
                        Message = $"SubCategory with ID '{id}' not found."
                    });
                }
                return Ok(new GeneralResponse<string> // Return 200 OK with a success message
                {
                    Success = true,
                    Message = "SubCategory deleted successfully."
                });
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
    }
}