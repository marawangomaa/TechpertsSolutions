using Core.DTOs.SubCategory;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization; // For [Authorize] attribute
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace TechpertsSolutions.API.Controllers
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
        /// <returns>A list of SubCategoryDto.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SubCategoryDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllSubCategories()
        {
            try
            {
                var subCategories = await _subCategoryService.GetAllSubCategoriesAsync();
                return Ok(subCategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all subcategories.");
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Gets a subcategory by its ID.
        /// </summary>
        /// <param name="id">The ID of the subcategory.</param>
        /// <returns>A SubCategoryDto if found, otherwise 404 Not Found.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SubCategoryDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSubCategoryById(string id)
        {
            try
            {
                var subCategory = await _subCategoryService.GetSubCategoryByIdAsync(id);
                if (subCategory == null)
                {
                    return NotFound($"SubCategory with ID '{id}' not found.");
                }
                return Ok(subCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting subcategory by ID: {id}");
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Gets subcategories by their parent Category ID.
        /// </summary>
        /// <param name="categoryId">The ID of the parent category.</param>
        /// <returns>A list of SubCategoryDto.</returns>
        [HttpGet("byCategory/{categoryId}")]
        [ProducesResponseType(typeof(IEnumerable<SubCategoryDTO>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSubCategoriesByCategoryId(string categoryId)
        {
            try
            {
                var subCategories = await _subCategoryService.GetSubCategoriesByCategoryIdAsync(categoryId);
                return Ok(subCategories);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting subcategories by Category ID: {categoryId}");
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Creates a new subcategory.
        /// </summary>
        /// <param name="createDto">The SubCategory data to create.</param>
        /// <returns>The created SubCategoryDto.</returns>
        [HttpPost]
        // [Authorize(Roles = "Admin,TechManager")] // Example: Only Admin or TechManager can create
        [ProducesResponseType(typeof(SubCategoryDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateSubCategory([FromBody] CreateSubCategoryDTO createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newSubCategory = await _subCategoryService.CreateSubCategoryAsync(createDto);
                return CreatedAtAction(nameof(GetSubCategoryById), new { id = newSubCategory.Id }, newSubCategory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subcategory.");
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Updates an existing subcategory.
        /// </summary>
        /// <param name="id">The ID of the subcategory to update.</param>
        /// <param name="updateDto">The updated SubCategory data.</param>
        /// <returns>204 No Content if successful, 400 Bad Request, 404 Not Found, or 500 Internal Server Error.</returns>
        [HttpPut("{id}")]
        // [Authorize(Roles = "Admin,TechManager")] // Example: Only Admin or TechManager can update
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateSubCategory(string id, [FromBody] UpdateSubCategoryDTO updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _subCategoryService.UpdateSubCategoryAsync(updateDto);
                if (!result)
                {
                    return NotFound($"SubCategory with ID '{id}' not found.");
                }
                return NoContent(); // 204 No Content for successful update
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating subcategory with ID: {id}");
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Deletes a subcategory by its ID.
        /// </summary>
        /// <param name="id">The ID of the subcategory to delete.</param>
        /// <returns>204 No Content if successful, 404 Not Found, or 500 Internal Server Error.</returns>
        [HttpDelete("{id}")]
        // [Authorize(Roles = "Admin,TechManager")] // Example: Only Admin or TechManager can delete
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteSubCategory(string id)
        {
            try
            {
                var result = await _subCategoryService.DeleteSubCategoryAsync(id);
                if (!result)
                {
                    return NotFound($"SubCategory with ID '{id}' not found.");
                }
                return NoContent(); // 204 No Content for successful deletion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting subcategory with ID: {id}");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}