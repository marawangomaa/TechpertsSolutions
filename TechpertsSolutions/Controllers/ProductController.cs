using Core.DTOs.ProductDTOs;
using Core.Enums;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _env;

        public ProductController(IProductService productService, IWebHostEnvironment env)
        {
            _productService = productService;
            _env = env;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] ProductPendingStatus? status = null,
            [FromQuery] string? categoryId = null,
            [FromQuery] string? subCategoryId = null,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = "name",
            [FromQuery] bool sortDesc = false)
        {
            var response = await _productService.GetAllAsync(pageNumber, pageSize, status, categoryId, subCategoryId, search, sortBy, sortDesc);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid product ID.",
                    Data = id
                });
            }

            var response = await _productService.GetByIdAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost]
        // [Authorize(Roles = "Admin,TechManager")]
        public async Task<IActionResult> AddProduct([FromForm] ProductCreateDTO dto, IFormFile? img)
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

            try
            {
                if (img != null)
                {
                    // Ensure uploads directory exists
                    var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "products");
                    Directory.CreateDirectory(uploadsDir);
                    
                    var fileName = $"{Guid.NewGuid()}_{img.FileName}";
                    var savePath = Path.Combine(uploadsDir, fileName);
                    using var stream = new FileStream(savePath, FileMode.Create);
                    await img.CopyToAsync(stream);
                    dto.ImageUrl = $"/uploads/products/{fileName}";
                }

                var response = await _productService.AddAsync(dto);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Failed to create product.",
                    Data = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin,TechManager")]
        public async Task<IActionResult> UpdateProduct(string id, [FromForm] ProductUpdateDTO dto, IFormFile? img)
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

            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid product ID.",
                    Data = id
                });
            }

            try
            {
                if (img != null)
                {
                    // Ensure uploads directory exists
                    var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "products");
                    Directory.CreateDirectory(uploadsDir);
                    
                    var fileName = $"{Guid.NewGuid()}_{img.FileName}";
                    var savePath = Path.Combine(uploadsDir, fileName);
                    using var stream = new FileStream(savePath, FileMode.Create);
                    await img.CopyToAsync(stream);
                    dto.ImageUrl = $"/uploads/products/{fileName}";
                }

                // Set the ID from the route parameter to ensure consistency
                dto.Id = id;
                var response = await _productService.UpdateAsync(id, dto);
                if (!response.Success)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Failed to update product.",
                    Data = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid product ID.",
                    Data = id
                });
            }

            var response = await _productService.DeleteAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
