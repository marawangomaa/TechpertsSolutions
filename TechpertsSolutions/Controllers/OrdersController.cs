using Core.DTOs.OrderDTOs;
using Core.Enums;
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
                Data = "Use cart checkout flow instead"
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
                    Data = "Missing Order ID"
                });
            }

            if (!Guid.TryParse(id, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid Order ID format. Expected GUID format.",
                    Data = "Invalid GUID format"
                });
            }

            var response = await _service.GetOrderByIdAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<GeneralResponse<IEnumerable<OrderReadDTO>>>> GetAll()
        {
            var response = await _service.GetAllOrdersAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
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
                    Data = "Missing Customer ID"
                });
            }

            if (!Guid.TryParse(customerId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Expected GUID format.",
                    Data = "Invalid GUID format"
                });
            }

            var response = await _service.GetOrdersByCustomerIdAsync(customerId);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("customer/{customerId}/history")]
        public async Task<IActionResult> GetOrderHistoryByCustomerId(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer ID cannot be null or empty.",
                    Data = "Invalid input"
                });

            if (!Guid.TryParse(customerId, out _))
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Must be a valid GUID.",
                    Data = "Invalid GUID format"
                });

            var response = await _service.GetOrderHistoryByCustomerIdAsync(customerId);
            return Ok(response);
        }
    }
}
