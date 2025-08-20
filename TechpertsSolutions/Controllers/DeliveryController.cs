using Azure;
using Core.DTOs;
using Core.DTOs.DeliveryDTOs;
using Core.Enums;
using Core.Interfaces.Services;
using Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Text.Json.Serialization;
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
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (!IsValidGuid(id))
                return BadRequest(InvalidIdResponse(id));

            var response = await _service.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DeliveryCreateDTO dto)
        {
            var response = await _service.CreateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("assign-driver")]
        public async Task<IActionResult> AssignDriverToCluster([FromQuery] string clusterId, [FromQuery] string driverId)
        {
            var response = await _service.AssignDriverToClusterAsync(clusterId, driverId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptDelivery([FromQuery] string clusterId, [FromQuery] string driverId)
        {
            var response = await _service.AcceptDeliveryAsync(clusterId, driverId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("decline")]
        public async Task<IActionResult> DeclineDelivery([FromQuery] string clusterId, [FromQuery] string driverId)
        {
            var response = await _service.DeclineDeliveryAsync(clusterId, driverId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("cancel/{deliveryId}")]
        public async Task<IActionResult> CancelDelivery(string deliveryId)
        {
            var response = await _service.CancelDeliveryAsync(deliveryId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("complete")]
        public async Task<IActionResult> CompleteDelivery([FromQuery] string deliveryId, [FromQuery] string driverId)
        {
            var response = await _service.CompleteDeliveryAsync(deliveryId, driverId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{clusterId}/status")]
        public async Task<IActionResult> UpdateClusterStatus(
            string clusterId,
            [FromBody] UpdateClusterStatusRequest request
        )
        {
            if (request == null)
                return BadRequest("Request body cannot be null.");

            var response = await _service.UpdateClusterStatusAsync(
                clusterId,
                request.Status,
                request.AssignedDriverId
            );

            return response.Success ? Ok(response) : BadRequest(response);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!IsValidGuid(id))
                return BadRequest(InvalidIdResponse(id));

            var response = await _service.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("tracking/{deliveryId}")]
        public async Task<IActionResult> GetDeliveryTracking(string deliveryId)
        {
            var response = await _service.GetDeliveryTrackingAsync(deliveryId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("expired-offers")]
        public async Task<IActionResult> GetDeliveriesWithExpiredOffers()
        {
            var deliveries = await _service.GetDeliveriesWithExpiredOffersAsync();
            return Ok(deliveries);
        }

        private bool IsValidGuid(string id) =>
            !string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out _);

        private GeneralResponse<string> InvalidIdResponse(string id) =>
            new GeneralResponse<string>
            {
                Success = false,
                Message = "Invalid or missing ID",
                Data = id
            };


        public class UpdateClusterStatusRequest
        {
            public DeliveryClusterStatus Status { get; set; }
            public string? AssignedDriverId { get; set; }
        }
    }
}