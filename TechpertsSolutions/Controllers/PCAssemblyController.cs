using Core.DTOs;
using Core.DTOs.PCAssemblyDTOs;
using Core.DTOs.CartDTOs;
using Core.DTOs.ProductDTOs;
using Core.Enums;
using Core.Interfaces.Services;
using Core.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.ComponentModel.DataAnnotations;
using TechpertsSolutions.Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PCAssemblyController : ControllerBase
    {
        private readonly IPCAssemblyService _pcAssemblyService;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        public PCAssemblyController(IPCAssemblyService pcAssemblyService, IProductService productService, ICartService cartService)
        {
            _pcAssemblyService = pcAssemblyService;
            _productService = productService;
            _cartService = cartService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PCAssemblyCreateDTO dto)
        {
            var result = await _pcAssemblyService.CreateAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _pcAssemblyService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _pcAssemblyService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(string customerId)
        {
            var result = await _pcAssemblyService.GetByCustomerIdAsync(customerId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PCAssemblyUpdateDTO dto)
        {
            var result = await _pcAssemblyService.UpdatePCAssemblyAsync(id, dto);
            return result.Success ? Ok(result) : NotFound(result);
        }


        [HttpGet("build/{assemblyId}/components")]
        public async Task<IActionResult> GetBuildComponents(string assemblyId)
        {
            var result = await _pcAssemblyService.GetAllComponentsAsync(assemblyId);
            return result.Success ? Ok(result) : NotFound(result);
        }



        [HttpGet("build/components/{category}")]
        public async Task<IActionResult> GetComponentsByCategory(
            ProductCategory category,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = "name",
            [FromQuery] bool sortDesc = false)
        {
            try
            {
                var response = await _productService.GetProductsByCategoryAsync(
                    category, pageNumber, pageSize, sortBy, sortDesc);
                
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
                    Message = "Failed to load components for category.",
                    Data = ex.Message
                });
            }
        }




        [HttpGet("build/categories")]
        public IActionResult GetPCComponentCategories()
        {
            var categories = Enum.GetValues(typeof(ProductCategory))
                .Cast<ProductCategory>()
                .Where(c => c != ProductCategory.PreBuildPC && c != ProductCategory.Laptop)
                .Select(c => new
                {
                    Value = c,
                    Name = c.GetStringValue(),
                    DisplayName = GetComponentDisplayName(c)
                })
                .ToList();

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "PC component categories retrieved successfully.",
                Data = categories
            });
        }

        private string GetComponentDisplayName(ProductCategory category)
        {
            return category switch
            {
                ProductCategory.Processor => "Processor",
                ProductCategory.Motherboard => "Motherboard",
                ProductCategory.CPUCooler => "CPU Cooler",
                ProductCategory.Case => "Case",
                ProductCategory.GraphicsCard => "Graphics Card",
                ProductCategory.RAM => "RAM",
                ProductCategory.Storage => "Storage",
                ProductCategory.CaseCooler => "Case Cooler",
                ProductCategory.PowerSupply => "Power Supply",
                ProductCategory.Monitor => "Monitor",
                ProductCategory.Accessories => "Accessories",
                _ => category.ToString()
            };
        }

        [HttpPost("build/{assemblyId}/add-component")]
        public async Task<IActionResult> AddComponentToBuild(string assemblyId, [FromForm] AddComponentToBuildDTO dto)
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
                var productResponse = await _productService.GetByIdAsync(dto.ProductId);
                if (!productResponse.Success || productResponse.Data == null)
                {
                    return BadRequest(new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "Product not found."
                    });
                }

                var product = productResponse.Data;

                // Replace existing component in the same category
                var existingComponent = await _pcAssemblyService.GetComponentByCategoryAsync(assemblyId, dto.Category);
                if (existingComponent != null)
                {
                    await _pcAssemblyService.RemoveComponentFromAssemblyAsync(assemblyId, existingComponent.ItemId);
                }

                var newItem = new PCAssemblyItemCreateDTO
                {
                    ProductId = dto.ProductId,
                    Quantity = 1,
                    Price = product.Price
                };

                var result = await _pcAssemblyService.AddComponentToAssemblyAsync(assemblyId, newItem);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Failed to add component to build.",
                    Data = ex.Message
                });
            }
        }

        [HttpDelete("build/{assemblyId}/remove-component/{itemId}")]
        public async Task<IActionResult> RemoveComponentFromBuild(string assemblyId, string itemId)
        {
            try
            {
                var result = await _pcAssemblyService.RemoveComponentFromAssemblyAsync(assemblyId, itemId);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Failed to remove component from build.",
                    Data = ex.Message
                });
            }
        }



        [HttpGet("build/{assemblyId}/status")]
        public async Task<IActionResult> GetPCBuildStatus(string assemblyId)
        {
            try
            {
                var result = await _pcAssemblyService.GetPCBuildStatusAsync(assemblyId);
                return result.Success ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Failed to get PC build status.",
                    Data = ex.Message
                });
            }
        }

        
        
        
        [HttpGet("build/{assemblyId}/total")]
        public async Task<IActionResult> GetPCBuildTotal(string assemblyId)
        {
            try
            {
                var result = await _pcAssemblyService.CalculateBuildTotalAsync(assemblyId);
                return result.Success ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Failed to calculate build total.",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("build/{assemblyId}/table")]
        public async Task<IActionResult> GetPCBuildTable(string assemblyId)
        {
            try
            {
                var result = await _pcAssemblyService.GetPCBuildTableAsync(assemblyId);
                return result.Success ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Failed to get PC build table.",
                    Data = ex.Message
                });
            }
        }




        [HttpGet("{assemblyId}/compatible-products/{categoryName}")]
        public async Task<IActionResult> GetCompatibleProductsForCategory(string assemblyId, string categoryName)
        {
            var response = await _pcAssemblyService.GetFilteredCompatibleComponentsAsync(assemblyId, categoryName);
            return Ok(response);
        }

        [HttpPost("build/{assemblyId}/add-to-cart")]
        public async Task<IActionResult> AddBuildToCart(string assemblyId, [FromQuery] decimal assemblyFee, [FromQuery] string customerId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customerId))
                {
                    return BadRequest(new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "Customer ID is required."
                    });
                }

                var result = await _pcAssemblyService.SaveBuildToCartAsync(assemblyId, customerId, assemblyFee, _cartService);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Failed to add PC build to cart.",
                    Data = ex.Message
                });
            }
        }

        [HttpPost("{assemblyId}/move-to-cart")]
        public async Task<IActionResult> MoveAssemblyToCart(string assemblyId)
        {
            var response = await _pcAssemblyService.MoveAssemblyToCartAsync(assemblyId, _cartService);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


    }
}
