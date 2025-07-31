using Core.DTOs.ProductDTOs;
using Core.Enums;
using Core.Interfaces.Services;
using Core.Utilities;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;
using Core.DTOs;

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
            [FromQuery] ProductCategory? categoryEnum = null,
            [FromQuery] string? subCategoryName = null,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = "name",
            [FromQuery] bool sortDesc = false)
        {
            var response = await _productService.GetAllAsync(pageNumber, pageSize, status, categoryEnum, subCategoryName, search, sortBy, sortDesc);
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

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDesc = false)
        {
            var response = await _productService.GetPendingProductsAsync(pageNumber, pageSize, sortBy, sortDesc);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetProductsByStatus(ProductPendingStatus status)
        {
            var response = await _productService.GetAllAsync(1, 100, status);
            return Ok(response);
        }

        [HttpGet("category/{categoryEnum}")]
        public async Task<IActionResult> GetProductsByCategory(
            [FromRoute] ProductCategory categoryEnum,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDesc = false)
        {
            var response = await _productService.GetProductsByCategoryAsync(categoryEnum, pageNumber, pageSize, sortBy, sortDesc);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductCreateDTO dto, [FromQuery] ProductCategory category, [FromQuery] ProductPendingStatus status)
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
                var response = await _productService.AddAsync(dto, category, status);
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
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] ProductUpdateDTO dto, [FromQuery] ProductCategory? category = null, [FromQuery] ProductPendingStatus? status = null)
        {
            // Debug: Log the incoming request
            if (dto == null)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Request body is null or invalid. Please ensure you're sending valid JSON.",
                    Data = "Expected ProductUpdateDTO object"
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var detailedErrors = ModelState.Keys
                    .Where(key => ModelState[key].Errors.Count > 0)
                    .Select(key => $"{key}: {string.Join(", ", ModelState[key].Errors.Select(e => e.ErrorMessage))}")
                    .ToList();
                
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Validation failed. Please check the following fields:",
                    Data = string.Join("; ", detailedErrors)
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
                var categorySelect = category ?? ProductCategory.Processor;
                var statusSelect = status ?? ProductPendingStatus.Pending;
                var response = await _productService.UpdateAsync(id, dto, categorySelect, statusSelect);
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

        [HttpPost("{productId}/upload-image")]
        
        public async Task<IActionResult> UploadProductImage(string productId, IFormFile imageFile)
        {
            if (string.IsNullOrWhiteSpace(productId) || !Guid.TryParse(productId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid product ID.",
                    Data = productId
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

            var response = await _productService.UploadProductImageAsync(imageFile, productId);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("{productId}/delete-image")]
        
        public async Task<IActionResult> DeleteProductImage(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId) || !Guid.TryParse(productId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid product ID.",
                    Data = productId
                });
            }

            var response = await _productService.DeleteProductImageAsync(productId);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
