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
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "ID must not be null or empty",
                    Data = "Invalid input"
                });
            }

            if (!Guid.TryParse(id, out Guid guidId))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "ID format is invalid",
                    Data = "Expected GUID"
                });
            }

            try
            {
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
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while retrieving delivery",
                    Data = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DeliveryCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid data provided",
                    Data = "Not Valid Entry"
                });
            }

            try
            {
                var created = await _service.AddAsync(dto);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] DeliveryCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid data provided",
                    Data = "Not Valid Entry"
                });
            }

            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid guidId))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "ID is missing or not a valid GUID",
                    Data = id
                });
            }

            try
            {
                var result = await _service.UpdateAsync(id, dto);
                if (!result)
                {
                    return NotFound(new GeneralResponse<string>
                    {
                        Success = false,
                        Message = $"Delivery with ID {id} not found",
                        Data = "Not Updated"
                    });
                }

                return Ok(new GeneralResponse<string>
                {
                    Success = true,
                    Message = "Delivery updated successfully",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Failed to update delivery",
                    Data = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid guidId))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "ID is missing or not a valid GUID",
                    Data = id
                });
            }

            try
            {
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
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Failed to delete delivery",
                    Data = ex.Message
                });
            }
        }
    }
}
