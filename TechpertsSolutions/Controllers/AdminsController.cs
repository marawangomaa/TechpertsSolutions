using Core.DTOs.AdminDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public AdminsController(IAdminService adminService, IProductService productService, IOrderService orderService)
        {
            _adminService = adminService;
            _productService = productService;
            _orderService = orderService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _adminService.GetAllAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "ID must not be null or empty.",
                    Data = "Invalid input"
                });

            if (!Guid.TryParse(id, out _))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid ID format. Must be a valid GUID.",
                    Data = "Invalid GUID format"
                });

            var response = await _adminService.GetByIdAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        // Product Management Endpoints
        [HttpGet("products/pending")]
        public async Task<IActionResult> GetPendingProducts()
        {
            var response = await _productService.GetPendingProductsAsync();
            return Ok(response);
        }

        [HttpPost("products/{productId}/approve")]
        public async Task<IActionResult> ApproveProduct(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Product ID cannot be null or empty.",
                    Data = "Invalid input"
                });

            if (!Guid.TryParse(productId, out _))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid Product ID format. Must be a valid GUID.",
                    Data = "Invalid GUID format"
                });

            var response = await _productService.ApproveProductAsync(productId);
            return Ok(response);
        }

        [HttpPost("products/{productId}/reject")]
        public async Task<IActionResult> RejectProduct(string productId, [FromBody] string reason)
        {
            if (string.IsNullOrWhiteSpace(productId))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Product ID cannot be null or empty.",
                    Data = "Invalid input"
                });

            if (!Guid.TryParse(productId, out _))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid Product ID format. Must be a valid GUID.",
                    Data = "Invalid GUID format"
                });

            if (string.IsNullOrWhiteSpace(reason))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Rejection reason cannot be null or empty.",
                    Data = "Invalid input"
                });

            var response = await _productService.RejectProductAsync(productId, reason);
            return Ok(response);
        }

        // Order Management Endpoints
        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var response = await _orderService.GetAllOrdersAsync();
            return Ok(response);
        }

        [HttpGet("orders/status/{status}")]
        public async Task<IActionResult> GetOrdersByStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Status cannot be null or empty.",
                    Data = "Invalid input"
                });

            var response = await _orderService.GetOrdersByStatusAsync(status);
            return Ok(response);
        }

        [HttpPut("orders/{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] string newStatus)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Order ID cannot be null or empty.",
                    Data = "Invalid input"
                });

            if (!Guid.TryParse(orderId, out _))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid Order ID format. Must be a valid GUID.",
                    Data = "Invalid GUID format"
                });

            if (string.IsNullOrWhiteSpace(newStatus))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "New status cannot be null or empty.",
                    Data = "Invalid input"
                });

            var response = await _orderService.UpdateOrderStatusAsync(orderId, newStatus);
            return Ok(response);
        }

        // Dashboard Statistics
        [HttpGet("dashboard/stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var pendingProducts = await _productService.GetPendingProductsAsync();
                var allOrders = await _orderService.GetAllOrdersAsync();
                var pendingOrders = await _orderService.GetOrdersByStatusAsync("Pending");
                var deliveredOrders = await _orderService.GetOrdersByStatusAsync("Delivered");

                var stats = new
                {
                    PendingProducts = pendingProducts.Success ? pendingProducts.Data?.TotalItems ?? 0 : 0,
                    TotalOrders = allOrders.Success ? allOrders.Data?.Count() ?? 0 : 0,
                    PendingOrders = pendingOrders.Success ? pendingOrders.Data?.Count() ?? 0 : 0,
                    DeliveredOrders = deliveredOrders.Success ? deliveredOrders.Data?.Count() ?? 0 : 0
                };

                return Ok(new GeneralResponse<object>
                {
                    Success = true,
                    Message = "Dashboard statistics retrieved successfully.",
                    Data = stats
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while retrieving dashboard statistics.",
                    Data = ex.Message
                });
            }
        }
    }
}
