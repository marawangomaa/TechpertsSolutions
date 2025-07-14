using Core.DTOs.Delivery;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryService _service;

        public DeliveryController(IDeliveryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var deliveries = await _service.GetAllAsync();
            return Ok(new GeneralResponse<IEnumerable<DeliveryDTO>>
            {
                Success = true,
                Message = "All deliveries fetched",
                Data = deliveries
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "ID must not be null or empty",
                    Data = "Invalid input"
                });
            }

            var delivery = await _service.GetByIdAsync(id);
            if (delivery == null)
            {
                return NotFound(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Delivery not found",
                    Data = id
                });
            }

            return Ok(new GeneralResponse<DeliveryDTO>
            {
                Success = true,
                Message = "Delivery found",
                Data = delivery
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DeliveryCreateDTO dto)
        {
            await _service.AddAsync(dto);
            return Ok(new GeneralResponse<string>
            {
                Success = true,
                Message = "Delivery created successfully",
                Data = null
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] DeliveryCreateDTO dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok(new GeneralResponse<string>
            {
                Success = true,
                Message = "Delivery updated successfully",
                Data = id
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeleteAsync(id);
            return Ok(new GeneralResponse<string>
            {
                Success = true,
                Message = "Delivery deleted successfully",
                Data = id
            });
        }
    }
}
