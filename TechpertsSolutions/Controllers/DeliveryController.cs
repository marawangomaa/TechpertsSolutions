using Core.DTOs.Delivery;
using Core.Interfaces;
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
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid or missing ID",
                    Data = id
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

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetDetails(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid or missing ID",
                    Data = id
                });
            }

            var details = await _service.GetDetailsByIdAsync(id);
            if (details == null)
            {
                return NotFound(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Delivery not found",
                    Data = id
                });
            }

            return Ok(new GeneralResponse<DeliveryDetailsDTO>
            {
                Success = true,
                Message = "Delivery details fetched",
                Data = details
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            try
            {
                var created = await _service.AddAsync();
                return Ok(new GeneralResponse<DeliveryDTO>
                {
                    Success = true,
                    Message = "Delivery created successfully",
                    Data = created
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Failed to create delivery",
                    Data = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid or missing ID",
                    Data = id
                });
            }

            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"Delivery with ID {id} not found",
                    Data = id
                });
            }

            return Ok(new GeneralResponse<string>
            {
                Success = true,
                Message = "Delivery deleted successfully",
                Data = id
            });
        }
    }
}
