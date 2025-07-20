using Core.DTOs.Orders;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;

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
        public async Task<ActionResult<OrderHistoryReadDTO>> GetHistory(string id)
        {
            return Ok(await _historyService.GetHistoryByIdAsync(id));
        }
    }
}
