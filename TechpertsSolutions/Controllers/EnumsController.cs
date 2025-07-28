using Core.Enums;
using Core.Utilities;
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
                ServiceType.PCBuild => "Custom PC building service",
                ServiceType.Maintenance => "Product maintenance and repair service",
                ServiceType.Delivery => "Product delivery service",
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
