using Core.DTOs.DeliveryDTOs;
using Core.Interfaces.Services;
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
        public async Task<IActionResult> Create([FromBody] DeliveryCreateDTO dto)
        {
            var response = await _service.AddAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] DeliveryUpdateDTO dto)
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

            var response = await _service.UpdateAsync(id, dto);
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
        public async Task<IActionResult> GetByStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Status cannot be null or empty",
                    Data = status
                });
            }

            var response = await _service.GetByStatusAsync(status);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
