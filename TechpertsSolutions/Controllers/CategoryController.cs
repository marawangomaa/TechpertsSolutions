using Core.DTOs; // Added to access GeneralResponse<T>
using Core.DTOs.Category;
using Core.Interfaces.Services; // Assuming ICategoryService is in this namespace
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic; // For IEnumerable
using System.Linq; // For LINQ operations if needed
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;

// Assuming your DTOs are accessible, e.g., in a "Core.DTOs" or similar namespace
// For this example, I'll assume they are in the same project or a referenced project.
// If not, you might need a 'using' statement like:
// using TechpertsSolutions.DTOs; // Or wherever your DTOs are located

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService; // Renamed to _categoryService for common C# naming conventions

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Retrieves all categories.
        /// </summary>
        /// <returns>A list of CategoryDto wrapped in a GeneralResponse.</returns>
        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResponse<IEnumerable<CategoryDTO>>))]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(new GeneralResponse<IEnumerable<CategoryDTO>>
            {
                Success = true,
                Message = "Categories retrieved successfully.",
                Data = categories
            });
        }

        /// <summary>
        /// Retrieves a category by its ID.
        /// </summary>
        /// <param name="Id">The ID of the category.</param>
        /// <returns>A single CategoryDto if found, otherwise NotFound, both wrapped in a GeneralResponse.</returns>
        [HttpGet("GetCategory/{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResponse<CategoryDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(GeneralResponse<CategoryDTO>))]
        public async Task<IActionResult> GetById(string Id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(Id);
            if (category == null)
            {
                return NotFound(new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = $"Category with ID {Id} not found.",
                    Data = null
                });
            }
            return Ok(new GeneralResponse<CategoryDTO>
            {
                Success = true,
                Message = "Category retrieved successfully.",
                Data = category
            });
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="categoryCreateDto">The category data to create.</param>
        /// <returns>The created CategoryDto with its new ID, wrapped in a GeneralResponse.</returns>
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GeneralResponse<CategoryDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(GeneralResponse<CategoryDTO>))]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDTO categoryCreateDto)
        {
            if (!ModelState.IsValid)
            {
                // For validation errors, you might want to return a more detailed message
                // or a list of errors in the GeneralResponse. For simplicity, we'll
                // just indicate failure and the default ModelState errors.
                return BadRequest(new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = "Validation failed.",
                    // Data = null, // Or you could pass ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            var createdCategory = await _categoryService.CreateCategoryAsync(categoryCreateDto);
            // It's good practice to return 201 Created with the location of the new resource
            return CreatedAtAction(nameof(GetById), new { Id = createdCategory.Id }, new GeneralResponse<CategoryDTO>
            {
                Success = true,
                Message = "Category created successfully.",
                Data = createdCategory
            });
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="Id">The ID of the category to update (from route).</param>
        /// <param name="categoryUpdateDto">The updated category data (from body).</param>
        /// <returns>A GeneralResponse indicating success or failure.</returns>
        [HttpPut("Update/{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResponse<object>))] // Changed to 200 OK as we're returning a body
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(GeneralResponse<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(GeneralResponse<object>))]
        public async Task<IActionResult> Update(string Id, [FromBody] CategoryUpdateDTO categoryUpdateDto)
        {
            if (Id != categoryUpdateDto.Id)
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Route ID and body ID do not match.",
                    Data = null
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = null
                });
            }

            var result = await _categoryService.UpdateCategoryAsync(categoryUpdateDto);
            if (!result)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Category with ID {Id} not found.",
                    Data = null
                });
            }
            return Ok(new GeneralResponse<object> // Changed to Ok as we're returning a body
            {
                Success = true,
                Message = "Category updated successfully.",
                Data = null // Or you could return the updated DTO if desired
            });
        }

        /// <summary>
        /// Deletes a category by its ID.
        /// </summary>
        /// <param name="Id">The ID of the category to delete.</param>
        /// <returns>A GeneralResponse indicating success or failure.</returns>
        [HttpDelete("Delete/{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResponse<object>))] // Changed to 200 OK as we're returning a body
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(GeneralResponse<object>))]
        public async Task<IActionResult> Delete(string Id)
        {
            var result = await _categoryService.DeleteCategoryAsync(Id);
            if (!result)
            {
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = $"Category with ID {Id} not found.",
                    Data = null
                });
            }
            return Ok(new GeneralResponse<object> // Changed to Ok as we're returning a body
            {
                Success = true,
                Message = "Category deleted successfully.",
                Data = null
            });
        }
    }
}