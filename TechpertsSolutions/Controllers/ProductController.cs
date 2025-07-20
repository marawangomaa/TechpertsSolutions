using Core.DTOs.Product;
using Core.Enums;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var result = await _productService.GetAllAsync(pageNumber, pageSize, status, categoryId,subCategoryId ,search, sortBy, sortDesc);

            return Ok(new GeneralResponse<PaginatedDTO<ProductCardDTO>>
            {
                Success = true,
                Message = "Products fetched successfully with pagination.",
                Data = result
            });
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

            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "Product not found.",
                        Data = id
                    });
                }

                return Ok(new GeneralResponse<ProductDTO>
                {
                    Success = true,
                    Message = "Product retrieved successfully.",
                    Data = product
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Server error occurred.",
                    Data = ex.Message
                });
            }
        }

        [HttpPost]
       // [Authorize(Roles = "Admin,TechManager")]
        public async Task<IActionResult> AddProduct([FromForm] ProductCreateDTO dto, IFormFile? img)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid model.",
                    Data = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
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

                var product = await _productService.AddAsync(dto);
                return Ok(new GeneralResponse<ProductDTO>
                {
                    Success = true,
                    Message = "Product added successfully.",
                    Data = product
                });
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
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid model.",
                    Data = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
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
                var updated = await _productService.UpdateAsync(id, dto);
                if (!updated)
                {
                    return NotFound(new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "Product not found.",
                        Data = id
                    });
                }

                return Ok(new GeneralResponse<string>
                {
                    Success = true,
                    Message = "Product updated successfully.",
                    Data = id
                });
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

            try
            {
                var deleted = await _productService.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound(new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "Product not found.",
                        Data = id
                    });
                }

                return Ok(new GeneralResponse<string>
                {
                    Success = true,
                    Message = "Product deleted successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Failed to delete product.",
                    Data = ex.Message
                });
            }
        }
    }
}
