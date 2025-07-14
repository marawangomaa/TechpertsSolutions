using Core.DTOs.Product;
using Core.Enums;
using Core.Interfaces;
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
            [FromQuery] ProductPendingStatus? status,
            [FromQuery] string? categoryId,
            [FromQuery] string? search,
            [FromQuery] string? sortBy,
            [FromQuery] bool sortDesc = false)
        {
            var result = await _productService.GetAllAsync(status, categoryId, search, sortBy, sortDesc);
            return Ok(new GeneralResponse<IEnumerable<ProductCardDTO>>
            {
                Success = true,
                Message = "Products fetched successfully.",
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(new GeneralResponse<string> { Success = false, Message = "Product not found", Data = id });

            return Ok(new GeneralResponse<ProductDTO>
            {
                Success = true,
                Message = "Product retrieved successfully.",
                Data = product
            });
        }

   
        [HttpPost]
        [Authorize(Roles = "Admin,TechManager")]
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

            if (img != null)
            {
                var fileName = $"{Guid.NewGuid()}_{img.FileName}";
                var savePath = Path.Combine(_env.WebRootPath, "uploads", "products", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
                using var stream = new FileStream(savePath, FileMode.Create);
                await img.CopyToAsync(stream);
                dto.ImgUrl = $"/uploads/products/{fileName}";
            }

            var product = await _productService.AddAsync(dto);
            return Ok(new GeneralResponse<ProductDTO>
            {
                Success = true,
                Message = "Product added successfully.",
                Data = product
            });
        }

     
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,TechManager")]
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

            if (img != null)
            {
                var fileName = $"{Guid.NewGuid()}_{img.FileName}";
                var savePath = Path.Combine(_env.WebRootPath, "uploads", "products", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
                using var stream = new FileStream(savePath, FileMode.Create);
                await img.CopyToAsync(stream);
                dto.ImgUrl = $"/uploads/products/{fileName}";
            }

            await _productService.UpdateAsync(id, dto);
            return Ok(new GeneralResponse<string>
            {
                Success = true,
                Message = "Product updated successfully.",
                Data = id
            });
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            await _productService.DeleteAsync(id);
            return Ok(new GeneralResponse<string>
            {
                Success = true,
                Message = "Product deleted successfully.",
                Data = id
            });
        }
    }
}
