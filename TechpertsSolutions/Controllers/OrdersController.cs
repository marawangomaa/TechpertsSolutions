using Core.DTOs.Orders;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;

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
        public async Task<ActionResult<OrderReadDTO>> CreateOrder(OrderCreateDTO dto)
        {
            var result = await _service.CreateOrderAsync(dto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderReadDTO>> GetOrder(string id)
        {
            var result = await _service.GetOrderByIdAsync(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderReadDTO>>> GetAll()
        {
            return Ok(await _service.GetAllOrdersAsync());
        }

        [HttpGet("by-customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<OrderReadDTO>>> GetByCustomer(string customerId)
        {
            return Ok(await _service.GetOrdersByCustomerIdAsync(customerId));
        }
    }
}
