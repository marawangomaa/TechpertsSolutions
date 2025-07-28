using Core.DTOs.DeliveryPersonDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryPersonController : ControllerBase
    {
        private readonly IDeliveryPersonService _deliveryPersonService;

        public DeliveryPersonController(IDeliveryPersonService deliveryPersonService)
        {
            _deliveryPersonService = deliveryPersonService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DeliveryPersonCreateDTO dto)
        {
            var result = await _deliveryPersonService.CreateAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
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

            var result = await _deliveryPersonService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _deliveryPersonService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable()
        {
            var result = await _deliveryPersonService.GetAvailableDeliveryPersonsAsync();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] DeliveryPersonUpdateDTO dto)
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

            var result = await _deliveryPersonService.UpdateAsync(id, dto);
            return result.Success ? Ok(result) : NotFound(result);
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

            var result = await _deliveryPersonService.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
} 
