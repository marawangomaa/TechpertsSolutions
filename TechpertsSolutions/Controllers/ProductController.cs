using Core.DTOs.ProductDTOs;
using Core.Enums;
using Core.Interfaces.Services;
using Core.Utilities;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;
using Core.DTOs;
using System.Text.Json;

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
        public async Task<IActionResult> AddProduct([FromBody] ProductCreateAllDTO MainDto,
                                                    ProductCategory category = ProductCategory.UnCategorized,
                                                    ProductPendingStatus status = ProductPendingStatus.Pending)
        {
            if(MainDto == null)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid product data.",
                    Data = "ProductCreateAllDTO is null"
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
                var response = await _productService.AddAsync(MainDto.product, MainDto.WarrantiesSpecs,category, status);
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
        public async Task<IActionResult> UpdateProduct([FromQuery] string Id, ProductPendingStatus Status, ProductCategory Category, [FromBody]ProductUpdateAllDTO MainDto)
        {
            // Debug: Log the incoming request
            if (MainDto == null)
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

            if (string.IsNullOrWhiteSpace(Id) || !Guid.TryParse(Id, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid product ID.",
                    Data = Id
                });
            }

            try
            {
                var categorySelect = Category;
                var statusSelect = Status;
                var response = await _productService.UpdateAsync(Id, MainDto.product, MainDto.WarrantiesSpecs,categorySelect, statusSelect);
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

        [HttpGet("tech-company")]
        public async Task<IActionResult> GetTechCompanyProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] ProductPendingStatus? status = null,
            [FromQuery] ProductCategory? categoryEnum = null,
            [FromQuery] string? subCategoryName = null,
            [FromQuery] string? nameSearch = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDescending = false,
            [FromQuery] string? techCompanyId = null)
        {
            var result = await _productService.GetAllTechCompanyProductAsync(
                pageNumber,
                pageSize,
                status,
                categoryEnum,
                subCategoryName,
                nameSearch,
                sortBy,
                sortDescending,
                techCompanyId
            );

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
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
        
        public async Task<IActionResult> UploadProductImage(string productId, ProductCreateImageUploadDTO imageUploadDto)
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

            if (imageUploadDto.ImageUrl == null || imageUploadDto.ImageUrl.Length == 0)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "No image file provided.",
                    Data = null
                });
            }

            var response = await _productService.UploadProductImageAsync(imageUploadDto, productId);
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
