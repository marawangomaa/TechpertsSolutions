using Core.DTOs.DeliveryDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Core.DTOs;
using Core.Enums;

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
            var response = await _service.GetAllAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
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

            var response = await _service.GetByIdAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
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

            var response = await _service.GetDetailsByIdAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DeliveryCreateDTO dto, [FromQuery] DeliveryStatus deliveryStatus = DeliveryStatus.Pending)
        {
            // Create a new DTO with the deliveryStatus from query parameter
            var dtoWithStatus = new DeliveryCreateDTO
            {
                TrackingNumber = dto.TrackingNumber,
                DeliveryAddress = dto.DeliveryAddress,
                CustomerPhone = dto.CustomerPhone,
                CustomerName = dto.CustomerName,
                EstimatedDeliveryDate = dto.EstimatedDeliveryDate,
                Notes = dto.Notes,
                DeliveryFee = dto.DeliveryFee,
                DeliveryPersonId = dto.DeliveryPersonId,
                CustomerId = dto.CustomerId
            };

            var response = await _service.AddAsync(dtoWithStatus);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] DeliveryUpdateDTO dto, [FromQuery] DeliveryStatus? deliveryStatus = null)
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

            // Create a new DTO with the deliveryStatus from query parameter if provided
            var dtoWithStatus = new DeliveryUpdateDTO
            {
                TrackingNumber = dto.TrackingNumber,
                DeliveryAddress = dto.DeliveryAddress,
                CustomerPhone = dto.CustomerPhone,
                CustomerName = dto.CustomerName,
                EstimatedDeliveryDate = dto.EstimatedDeliveryDate,
                ActualDeliveryDate = dto.ActualDeliveryDate,
                DeliveryStatus = deliveryStatus?.ToString() ?? dto.DeliveryStatus,
                Notes = dto.Notes,
                DeliveryFee = dto.DeliveryFee,
                DeliveryPersonId = dto.DeliveryPersonId
            };

            var response = await _service.UpdateAsync(id, dtoWithStatus);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
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

            var response = await _service.DeleteAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("deliveryperson/{deliveryPersonId}")]
        public async Task<IActionResult> GetByDeliveryPersonId(string deliveryPersonId)
        {
            if (string.IsNullOrWhiteSpace(deliveryPersonId) || !Guid.TryParse(deliveryPersonId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid or missing delivery person ID",
                    Data = deliveryPersonId
                });
            }

            var response = await _service.GetByDeliveryPersonIdAsync(deliveryPersonId);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus([FromRoute] DeliveryStatus status)
        {
            var response = await _service.GetByStatusAsync(status.ToString());
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
