using Core.DTOs.Orders;
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

            var history = await _historyService.GetHistoryByIdAsync(id);
            if (history == null || string.IsNullOrEmpty(history.Id))
            {
                return NotFound(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Order history not found.",
                    Data = id
                });
            }

            return Ok(new GeneralResponse<OrderHistoryReadDTO>
            {
                Success = true,
                Message = "Order history retrieved successfully.",
                Data = history
            });
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

            var history = await _historyService.GetHistoryByCustomerIdAsync(customerId);
            return Ok(new GeneralResponse<OrderHistoryReadDTO>
            {
                Success = true,
                Message = "Customer order history retrieved successfully.",
                Data = history
            });
        }

        [HttpGet]
        public async Task<ActionResult<GeneralResponse<IEnumerable<OrderHistoryReadDTO>>>> GetAllHistories()
        {
            var histories = await _historyService.GetAllOrderHistoriesAsync();
            return Ok(new GeneralResponse<IEnumerable<OrderHistoryReadDTO>>
            {
                Success = true,
                Message = "All order histories retrieved successfully.",
                Data = histories
            });
        }
    }
}
