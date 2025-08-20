using Core.DTOs;
using Core.DTOs.DeliveryPersonDTOs;
using Core.Enums;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechpertsSolutions.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin,DeliveryPerson")]
    public class DeliveryPersonController : ControllerBase
    {
        private readonly IDeliveryPersonService _deliveryPersonService;

        public DeliveryPersonController(IDeliveryPersonService deliveryPersonService)
        {
            _deliveryPersonService = deliveryPersonService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (IsInvalidGuid(id, out var errorResponse))
                return BadRequest(errorResponse);

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
        public async Task<IActionResult> Update(string id, DeliveryPersonStatus AccountStatus, [FromBody] DeliveryPersonUpdateDTO dto)
        {
            if (IsInvalidGuid(id, out var errorResponse))
                return BadRequest(errorResponse);

            if (dto == null)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Request body cannot be null.",
                    Data = null
                });
            }

            var result = await _deliveryPersonService.UpdateAsync(id, AccountStatus, dto);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("{driverId}/offers/all")]
        public async Task<IActionResult> GetAllOffers(string driverId)
        {
            if (IsInvalidGuid(driverId, out var errorResponse))
                return BadRequest(errorResponse);

            var result = await _deliveryPersonService.GetAllOffersAsync(driverId);
            return Ok(result);
        }

        [HttpGet("{driverId}/offers/pending")]
        public async Task<IActionResult> GetPendingOffers(string driverId)
        {
            if (IsInvalidGuid(driverId, out var errorResponse))
                return BadRequest(errorResponse);

            var result = await _deliveryPersonService.GetPendingOffersAsync(driverId);
            return Ok(result);
        }

        [HttpPost("{driverId}/offers/{offerId}/accept")]
        public async Task<IActionResult> AcceptOffer(string driverId, string offerId)
        {
            if (IsInvalidGuid(driverId, out var errorResponse) || IsInvalidGuid(offerId, out errorResponse))
                return BadRequest(errorResponse);

            var result = await _deliveryPersonService.AcceptOfferAsync(offerId, driverId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{driverId}/offers/{offerId}/decline")]
        public async Task<IActionResult> DeclineOffer(string driverId, string offerId)
        {
            if (IsInvalidGuid(driverId, out var errorResponse) || IsInvalidGuid(offerId, out errorResponse))
                return BadRequest(errorResponse);

            var result = await _deliveryPersonService.DeclineOfferAsync(offerId, driverId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{driverId}/offers/{offerId}/cancel")]
        public async Task<IActionResult> CancelOffer(string driverId, string offerId)
        {
            if (IsInvalidGuid(driverId, out var errorResponse) || IsInvalidGuid(offerId, out errorResponse))
                return BadRequest(errorResponse);

            var result = await _deliveryPersonService.CancelOfferAsync(offerId, driverId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // --- Helpers ---
        private bool IsInvalidGuid(string id, out GeneralResponse<string> errorResponse)
        {
            errorResponse = null;
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
            {
                errorResponse = new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid or missing ID. Expected GUID format.",
                    Data = id
                };
                return true;
            }
            return false;
        }
    }
}
