using Core.DTOs.OrderDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using TechpertsSolutions.Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderHistoriesController : ControllerBase
    {
        private readonly IOrderHistoryService _historyService;

        public OrderHistoriesController(IOrderHistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GeneralResponse<OrderHistoryReadDTO>>> GetHistory(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "History ID cannot be null or empty.",
                    Data = null
                });
            }

            var response = await _historyService.GetHistoryByIdAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<GeneralResponse<OrderHistoryReadDTO>>> GetHistoryByCustomer(string customerId)
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

            var response = await _historyService.GetHistoryByCustomerIdAsync(customerId);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<GeneralResponse<IEnumerable<OrderHistoryReadDTO>>>> GetAllHistories()
        {
            var response = await _historyService.GetAllOrderHistoriesAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
