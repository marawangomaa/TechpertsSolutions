using Core.DTOs.CategoryDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;
using Core.DTOs;

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

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GeneralResponse<CategoryDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(GeneralResponse<CategoryDTO>))]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDTO categoryCreateDto)
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

        [HttpPost("{categoryId}/upload-image")]
        public async Task<IActionResult> UploadCategoryImage(string categoryId, IFormFile imageFile)
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

            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "No image file provided.",
                    Data = null
                });
            }

            var response = await _categoryService.UploadCategoryImageAsync(imageFile, categoryId);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("{categoryId}/delete-image")]
        public async Task<IActionResult> DeleteCategoryImage(string categoryId)
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

            var response = await _categoryService.DeleteCategoryImageAsync(categoryId);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}