using Core.DTOs.Orders;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using TechpertsSolutions.Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        /// <summary>
        /// DEPRECATED: Direct order creation is not supported. 
        /// Please use the cart checkout flow instead:
        /// 1. Add items to cart: POST /api/cart/{customerId}/items
        /// 2. Checkout from cart: POST /api/cart/{customerId}/checkout
        /// </summary>
        [HttpPost]
        public ActionResult<GeneralResponse<string>> CreateOrder(OrderCreateDTO dto)
        {
            return BadRequest(new GeneralResponse<string>
            {
                Success = false,
                Message = "Direct order creation is not supported. Please use the cart checkout flow:\n" +
                         "1. Add items to cart: POST /api/cart/{customerId}/items\n" +
                         "2. Checkout from cart: POST /api/cart/{customerId}/checkout\n" +
                         "Or use advanced checkout: POST /api/cart/checkout",
                Data = null
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GeneralResponse<OrderReadDTO>>> GetOrder(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Order ID cannot be null or empty.",
                    Data = null
                });
            }

            if (!Guid.TryParse(id, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid Order ID format. Expected GUID format.",
                    Data = null
                });
            }

            var result = await _service.GetOrderByIdAsync(id);
            if (result == null)
            {
                return NotFound(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Order not found.",
                    Data = id
                });
            }

            return Ok(new GeneralResponse<OrderReadDTO>
            {
                Success = true,
                Message = "Order retrieved successfully.",
                Data = result
            });
        }

        [HttpGet]
        public async Task<ActionResult<GeneralResponse<IEnumerable<OrderReadDTO>>>> GetAll()
        {
            var orders = await _service.GetAllOrdersAsync();
            return Ok(new GeneralResponse<IEnumerable<OrderReadDTO>>
            {
                Success = true,
                Message = "Orders retrieved successfully.",
                Data = orders
            });
        }

        [HttpGet("by-customer/{customerId}")]
        public async Task<ActionResult<GeneralResponse<IEnumerable<OrderReadDTO>>>> GetByCustomer(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer ID cannot be null or empty.",
                    Data = null
                });
            }

            if (!Guid.TryParse(customerId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Expected GUID format.",
                    Data = null
                });
            }

            var orders = await _service.GetOrdersByCustomerIdAsync(customerId);
            return Ok(new GeneralResponse<IEnumerable<OrderReadDTO>>
            {
                Success = true,
                Message = "Customer orders retrieved successfully.",
                Data = orders
            });
        }
    }
}
