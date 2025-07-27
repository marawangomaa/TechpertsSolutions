using Core.DTOs.CategoryDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

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
            var response = await _categoryService.GetAllCategoriesAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
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
            var response = await _categoryService.GetCategoryByIdAsync(Id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="categoryCreateDto">The category data to create.</param>
        /// <returns>The created CategoryDto with its new ID, wrapped in a GeneralResponse.</returns>
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GeneralResponse<CategoryDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(GeneralResponse<CategoryDTO>))]
        public async Task<IActionResult> Create([FromForm] CategoryCreateDTO categoryCreateDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new GeneralResponse<CategoryDTO>
                {
                    Success = false,
                    Message = "Validation failed: " + string.Join("; ", errors),
                    Data = null
                });
            }

            var response = await _categoryService.CreateCategoryAsync(categoryCreateDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return CreatedAtAction(nameof(GetById), new { Id = response.Data?.Id }, response);
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="Id">The ID of the category to update (from route).</param>
        /// <param name="categoryUpdateDto">The updated category data (from body).</param>
        /// <returns>A GeneralResponse indicating success or failure.</returns>
        [HttpPut("Update/{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResponse<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(GeneralResponse<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(GeneralResponse<object>))]
        public async Task<IActionResult> Update(string Id, [FromForm] CategoryUpdateDTO categoryUpdateDto)
        {
            if (Id != categoryUpdateDto.Id)
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Route ID and body ID do not match.",
                    Data = "Route ID: " + Id + ", Body ID: " + categoryUpdateDto.Id
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Validation failed: " + string.Join("; ", errors),
                    Data = "Invalid form data"
                });
            }

            var response = await _categoryService.UpdateCategoryAsync(categoryUpdateDto);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Deletes a category by its ID.
        /// </summary>
        /// <param name="Id">The ID of the category to delete.</param>
        /// <returns>A GeneralResponse indicating success or failure.</returns>
        [HttpDelete("Delete/{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResponse<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(GeneralResponse<object>))]
        public async Task<IActionResult> Delete(string Id)
        {
            var response = await _categoryService.DeleteCategoryAsync(Id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}