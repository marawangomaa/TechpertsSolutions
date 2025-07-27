using Core.DTOs.PCAssemblyDTOs;
using Core.DTOs.ProductDTOs;
using Core.Enums;
using Core.Interfaces.Services;
using Core.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public PCAssemblyController(IPCAssemblyService pcAssemblyService, IProductService productService)
        {
            _pcAssemblyService = pcAssemblyService;
            _productService = productService;
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

        // New endpoints for PC Build functionality

        /// <summary>
        /// Get products by PC component category for the PC Build page
        /// </summary>
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

        /// <summary>
        /// Get all available PC component categories
        /// </summary>
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

        /// <summary>
        /// Add a component to an existing PC build
        /// </summary>
        [HttpPost("build/{assemblyId}/add-component")]
        public async Task<IActionResult> AddComponentToBuild(
            string assemblyId,
            [FromBody] AddComponentToBuildDTO dto)
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
                // First, verify the product exists and belongs to the correct category
                var productResponse = await _productService.GetByIdAsync(dto.ProductId);
                if (!productResponse.Success)
                {
                    return BadRequest(new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "Product not found."
                    });
                }

                // Create the assembly item
                var assemblyItem = new PCAssemblyItemCreateDTO
                {
                    ProductId = dto.ProductId,
                    Quantity = 1,
                    Price = productResponse.Data?.Price ?? 0
                };

                // Add to existing assembly or create new one
                var result = await _pcAssemblyService.AddComponentToAssemblyAsync(assemblyId, assemblyItem);
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

        /// <summary>
        /// Remove a component from PC build
        /// </summary>
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

        /// <summary>
        /// Get current PC build status with component details
        /// </summary>
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

        /// <summary>
        /// Calculate total price of PC build
        /// </summary>
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

        /// <summary>
        /// Get compatible components for a specific component (for recommendations)
        /// </summary>
        [HttpGet("build/compatible/{productId}")]
        public async Task<IActionResult> GetCompatibleComponents(string productId)
        {
            try
            {
                var result = await _pcAssemblyService.GetCompatibleComponentsAsync(productId);
                return result.Success ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Failed to get compatible components.",
                    Data = ex.Message
                });
            }
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
    }

    // DTO for adding component to build
    public class AddComponentToBuildDTO
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;
    }
}
