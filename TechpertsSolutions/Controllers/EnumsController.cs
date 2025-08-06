using Core.DTOs;
using Core.DTOs.SubCategoryDTOs;
using Core.Enums;
using Core.Enums.BrandsEnums;
using Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;
using TechpertsSolutions.Core.DTOs.EnumDTOs;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnumsController : ControllerBase
    {
        [HttpGet("product-pending-status")]
        public IActionResult GetProductPendingStatusOptions()
        {
            var statusOptions = Enum.GetValues(typeof(ProductPendingStatus))
                .Cast<ProductPendingStatus>()
                .Select(status => new EnumOptionDTO
                {
                    Value = status.ToString(),
                    DisplayName = status.GetStringValue(),
                    Description = GetProductPendingStatusDescription(status)
                })
                .ToList();

            return Ok(new GeneralResponse<List<EnumOptionDTO>>
            {
                Success = true,
                Message = "Product pending status options retrieved successfully.",
                Data = statusOptions
            });
        }

        [HttpGet("service-types")]
        public IActionResult GetServiceTypeOptions()
        {
            var serviceTypeOptions = Enum.GetValues(typeof(ServiceType))
                .Cast<ServiceType>()
                .Select(serviceType => new EnumOptionDTO
                {
                    Value = serviceType.ToString(),
                    DisplayName = serviceType.GetStringValue(),
                    Description = GetServiceTypeDescription(serviceType)
                })
                .ToList();

            return Ok(new GeneralResponse<List<EnumOptionDTO>>
            {
                Success = true,
                Message = "Service type options retrieved successfully.",
                Data = serviceTypeOptions
            });
        }

        [HttpGet("product-categories")]
        public IActionResult GetProductCategoryOptions()
        {
            var categoryOptions = Enum.GetValues(typeof(ProductCategory))
                .Cast<ProductCategory>()
                .Select(category => new EnumOptionDTO
                {
                    Value = category.ToString(),
                    DisplayName = category.GetStringValue(),
                    Description = GetProductCategoryDescription(category)
                })
                .ToList();

            return Ok(new GeneralResponse<List<EnumOptionDTO>>
            {
                Success = true,
                Message = "Product category options retrieved successfully.",
                Data = categoryOptions
            });
        }

        [HttpGet("order-status")]
        public IActionResult GetOrderStatusOptions()
        {
            var statusOptions = Enum.GetValues(typeof(OrderStatus))
                .Cast<OrderStatus>()
                .Select(status => new EnumOptionDTO
                {
                    Value = status.ToString(),
                    DisplayName = status.GetStringValue(),
                    Description = GetOrderStatusDescription(status)
                })
                .ToList();

            return Ok(new GeneralResponse<List<EnumOptionDTO>>
            {
                Success = true,
                Message = "Order status options retrieved successfully.",
                Data = statusOptions
            });
        }

        [HttpGet("all")]
        public IActionResult GetAllEnumOptions()
        {
            var allEnums = new
            {
                ProductPendingStatus = Enum.GetValues(typeof(ProductPendingStatus))
                    .Cast<ProductPendingStatus>()
                    .Select(status => new { Value = status.ToString(), DisplayName = status.GetStringValue() })
                    .ToList(),
                ServiceType = Enum.GetValues(typeof(ServiceType))
                    .Cast<ServiceType>()
                    .Select(serviceType => new { Value = serviceType.ToString(), DisplayName = serviceType.GetStringValue() })
                    .ToList(),
                ProductCategory = Enum.GetValues(typeof(ProductCategory))
                    .Cast<ProductCategory>()
                    .Select(category => new { Value = category.ToString(), DisplayName = category.GetStringValue() })
                    .ToList(),
                RoleType = Enum.GetValues(typeof(RoleType))
                    .Cast<RoleType>()
                    .Select(role => new { Value = role.ToString(), DisplayName = role.GetStringValue() })
                    .ToList(),
                OrderStatus = Enum.GetValues(typeof(OrderStatus))
                    .Cast<OrderStatus>()
                    .Select(status => new { Value = status.ToString(), DisplayName = status.GetStringValue() })
                    .ToList()
            };

            return Ok(new GeneralResponse<dynamic>
            {
                Success = true,
                Message = "All enum options retrieved successfully.",
                Data = allEnums
            });
        }

        [HttpGet("subcategories-by-category")]
        public IActionResult GetSubCategoriesByCategory()
        {
            var subCategoriesByCategory = new
            {
                Processor = Enum.GetValues(typeof(ProcessorBrands))
                    .Cast<ProcessorBrands>()
                    .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),
                Motherboard = Enum.GetValues(typeof(MotherboardBrands))
                    .Cast<MotherboardBrands>()
                    .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),
                GraphicsCard = Enum.GetValues(typeof(GraphicsCardBrands))
                    .Cast<GraphicsCardBrands>()
                    .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),
                RAM = Enum.GetValues(typeof(RAMBrands))
                    .Cast<RAMBrands>()
                    .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),
                Storage = Enum.GetValues(typeof(StorageBrands))
                    .Cast<StorageBrands>()
                    .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),
                CPUCooler = Enum.GetValues(typeof(CpuCoolerBrands))
                    .Cast<CpuCoolerBrands>()
                    .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),
                Case = Enum.GetValues(typeof(CaseBrands))
                    .Cast<CaseBrands>()
                    .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),
                CaseCooler = Enum.GetValues(typeof(CaseCoolerBrands))
                    .Cast<CaseCoolerBrands>()
                    .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),
                PowerSupply = Enum.GetValues(typeof(PowerSupplyBrands))
                    .Cast<PowerSupplyBrands>()
                    .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),
                Monitor = Enum.GetValues(typeof(MonitorBrands))
                    .Cast<MonitorBrands>()
                    .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),
                Accessories = Enum.GetValues(typeof(AccessoryBrands))
                    .Cast<AccessoryBrands>()
                    .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),
                PreBuildPC = Enum.GetValues(typeof(PrebuiltPcBrands))
                    .Cast<PrebuiltPcBrands>()
                    .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),
                Laptop = Enum.GetValues(typeof(LaptopBrands))
                    .Cast<LaptopBrands>()
                    .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList()
            };

            return Ok(new GeneralResponse<dynamic>
            {
                Success = true,
                Message = "Subcategories by category retrieved successfully.",
                Data = subCategoriesByCategory
            });
        }

        [HttpGet("subcategories/{category}")]
        public IActionResult GetSubCategoriesByCategoryEnum(string category)
        {
            if (!Enum.TryParse<ProductCategory>(category, true, out var categoryEnum))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid category enum value.",
                    Data = category
                });
            }

            List<SubCategoryEnumDTO> subCategories = categoryEnum switch
            {
                ProductCategory.Processor => Enum.GetValues(typeof(ProcessorBrands))
                    .Cast<ProcessorBrands>()
                    .Select(brand => new SubCategoryEnumDTO { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),

                ProductCategory.Motherboard => Enum.GetValues(typeof(MotherboardBrands))
                    .Cast<MotherboardBrands>()
                    .Select(brand => new SubCategoryEnumDTO { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),

                ProductCategory.GraphicsCard => Enum.GetValues(typeof(GraphicsCardBrands))
                    .Cast<GraphicsCardBrands>()
                    .Select(brand => new SubCategoryEnumDTO { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),

                ProductCategory.RAM => Enum.GetValues(typeof(RAMBrands))
                    .Cast<RAMBrands>()
                    .Select(brand => new SubCategoryEnumDTO { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),

                ProductCategory.Storage => Enum.GetValues(typeof(StorageBrands))
                    .Cast<StorageBrands>()
                    .Select(brand => new SubCategoryEnumDTO { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),

                ProductCategory.CPUCooler => Enum.GetValues(typeof(CpuCoolerBrands))
                    .Cast<CpuCoolerBrands>()
                    .Select(brand => new SubCategoryEnumDTO { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),

                ProductCategory.Case => Enum.GetValues(typeof(CaseBrands))
                    .Cast<CaseBrands>()
                    .Select(brand => new SubCategoryEnumDTO { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),

                ProductCategory.CaseCooler => Enum.GetValues(typeof(CaseCoolerBrands))
                    .Cast<CaseCoolerBrands>()
                    .Select(brand => new SubCategoryEnumDTO { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),

                ProductCategory.PowerSupply => Enum.GetValues(typeof(PowerSupplyBrands))
                    .Cast<PowerSupplyBrands>()
                    .Select(brand => new SubCategoryEnumDTO { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),

                ProductCategory.Monitor => Enum.GetValues(typeof(MonitorBrands))
                    .Cast<MonitorBrands>()
                    .Select(brand => new SubCategoryEnumDTO { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),

                ProductCategory.Accessories => Enum.GetValues(typeof(AccessoryBrands))
                    .Cast<AccessoryBrands>()
                    .Select(brand => new SubCategoryEnumDTO { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),

                ProductCategory.PreBuildPC => Enum.GetValues(typeof(PrebuiltPcBrands))
                    .Cast<PrebuiltPcBrands>()
                    .Select(brand => new SubCategoryEnumDTO { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),

                ProductCategory.Laptop => Enum.GetValues(typeof(LaptopBrands))
                    .Cast<LaptopBrands>()
                    .Select(brand => new SubCategoryEnumDTO { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                    .ToList(),

                _ => new List<SubCategoryEnumDTO>()
            };

            return Ok(new GeneralResponse<dynamic>
            {
                Success = true,
                Message = $"Subcategories for {category} retrieved successfully.",
                Data = subCategories
            });
        }

        [HttpGet("categories-with-subcategories")]
        public IActionResult GetCategoriesWithSubCategories()
        {
            var categoriesWithSubCategories = new
            {
                Categories = Enum.GetValues(typeof(ProductCategory))
                    .Cast<ProductCategory>()
                    .Where(c => c != ProductCategory.UnCategorized)
                    .Select(category => new 
                    { 
                        Value = category.ToString(), 
                        DisplayName = category.GetStringValue(),
                        Description = GetProductCategoryDescription(category)
                    })
                    .ToList(),
                SubCategoriesByCategory = new
                {
                    Processor = Enum.GetValues(typeof(ProcessorBrands))
                        .Cast<ProcessorBrands>()
                        .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                        .ToList(),
                    Motherboard = Enum.GetValues(typeof(MotherboardBrands))
                        .Cast<MotherboardBrands>()
                        .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                        .ToList(),
                    GraphicsCard = Enum.GetValues(typeof(GraphicsCardBrands))
                        .Cast<GraphicsCardBrands>()
                        .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                        .ToList(),
                    RAM = Enum.GetValues(typeof(RAMBrands))
                        .Cast<RAMBrands>()
                        .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                        .ToList(),
                    Storage = Enum.GetValues(typeof(StorageBrands))
                        .Cast<StorageBrands>()
                        .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                        .ToList(),
                    CPUCooler = Enum.GetValues(typeof(CpuCoolerBrands))
                        .Cast<CpuCoolerBrands>()
                        .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                        .ToList(),
                    Case = Enum.GetValues(typeof(CaseBrands))
                        .Cast<CaseBrands>()
                        .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                        .ToList(),
                    CaseCooler = Enum.GetValues(typeof(CaseCoolerBrands))
                        .Cast<CaseCoolerBrands>()
                        .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                        .ToList(),
                    PowerSupply = Enum.GetValues(typeof(PowerSupplyBrands))
                        .Cast<PowerSupplyBrands>()
                        .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                        .ToList(),
                    Monitor = Enum.GetValues(typeof(MonitorBrands))
                        .Cast<MonitorBrands>()
                        .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                        .ToList(),
                    Accessories = Enum.GetValues(typeof(AccessoryBrands))
                        .Cast<AccessoryBrands>()
                        .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                        .ToList(),
                    PreBuildPC = Enum.GetValues(typeof(PrebuiltPcBrands))
                        .Cast<PrebuiltPcBrands>()
                        .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                        .ToList(),
                    Laptop = Enum.GetValues(typeof(LaptopBrands))
                        .Cast<LaptopBrands>()
                        .Select(brand => new { Value = brand.ToString(), DisplayName = brand.GetStringValue() })
                        .ToList()
                }
            };

            return Ok(new GeneralResponse<dynamic>
            {
                Success = true,
                Message = "Categories with subcategories retrieved successfully.",
                Data = categoriesWithSubCategories
            });
        }

        private string GetProductPendingStatusDescription(ProductPendingStatus status)
        {
            return status switch
            {
                ProductPendingStatus.Pending => "Product is pending approval",
                ProductPendingStatus.Approved => "Product has been approved",
                ProductPendingStatus.Rejected => "Product has been rejected",
                _ => "Unknown status"
            };
        }

        private string GetServiceTypeDescription(ServiceType serviceType)
        {
            return serviceType switch
            {
                ServiceType.PCAssembly => "Custom PC building service",
                ServiceType.Maintenance => "Product maintenance and repair service",
                ServiceType.Delivery => "Product delivery service",
                ServiceType.ProductSale => "Product sales service",
                _ => "Unknown service type"
            };
        }

        private string GetProductCategoryDescription(ProductCategory category)
        {
            return category switch
            {
                ProductCategory.Processor => "Central Processing Units (CPUs)",
                ProductCategory.Motherboard => "Motherboards and mainboards",
                ProductCategory.CPUCooler => "CPU cooling solutions",
                ProductCategory.Case => "Computer cases and enclosures",
                ProductCategory.GraphicsCard => "Graphics cards and GPUs",
                ProductCategory.RAM => "Random Access Memory modules",
                ProductCategory.Storage => "Storage devices and drives",
                ProductCategory.CaseCooler => "Case cooling fans and systems",
                ProductCategory.PowerSupply => "Power supply units",
                ProductCategory.Monitor => "Computer monitors and displays",
                ProductCategory.Accessories => "Computer accessories and peripherals",
                ProductCategory.PreBuildPC => "Pre-built desktop computers",
                ProductCategory.Laptop => "Portable computers and laptops",
                _ => "Unknown category"
            };
        }

        private string GetOrderStatusDescription(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "Order is pending and waiting to be processed",
                OrderStatus.InProgress => "Order is currently being processed and prepared",
                OrderStatus.Delivered => "Order has been successfully delivered to the customer",
                _ => "Unknown order status"
            };
        }
    }
} 
