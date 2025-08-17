using Core.DTOs.DeliveryDTOs;
using Core.DTOs.DeliveryPersonDTOs;
using Core.Enums;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TechpertsSolutions.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryClusterController : ControllerBase
    {
        private readonly IDeliveryClusterService _clusterService;

        public DeliveryClusterController(IDeliveryClusterService clusterService)
        {
            _clusterService = clusterService;
        }

        [HttpGet("{clusterId}")]
        public async Task<IActionResult> GetById(string clusterId)
        {
            var result = await _clusterService.GetByIdAsync(clusterId);
            if (!result.Success) return NotFound(result.Message);
            return Ok(result.Data);
        }

        [HttpGet("delivery/{deliveryId}")]
        public async Task<IActionResult> GetByDeliveryId(string deliveryId)
        {
            var result = await _clusterService.GetByDeliveryIdAsync(deliveryId);
            if (!result.Success) return NotFound(result.Message);
            return Ok(result.Data);
        }

        [HttpPost("{deliveryId}")]
        public async Task<IActionResult> Create(string deliveryId, [FromBody] DeliveryClusterCreateDTO dto)
        {
            var result = await _clusterService.CreateClusterAsync(deliveryId, dto);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Data);
        }

        [HttpPut("{clusterId}")]
        public async Task<IActionResult> Update(string clusterId, [FromBody] DeliveryClusterDTO dto)
        {
            var result = await _clusterService.UpdateClusterAsync(clusterId, dto);
            if (!result.Success) return NotFound(result.Message);
            return Ok(result.Data);
        }

        [HttpGet("{clusterId}/tracking")]
        public async Task<IActionResult> GetTracking(string clusterId)
        {
            var result = await _clusterService.GetTrackingAsync(clusterId);
            if (!result.Success) return NotFound(result.Message);
            return Ok(result.Data);
        }

        [HttpPatch("{clusterId}/tracking")]
        public async Task<IActionResult> UpdateTracking(string clusterId, [FromBody] DeliveryClusterTrackingDTO dto)
        {
            var result = await _clusterService.UpdateClusterTrackingAsync(clusterId, dto);
            if (!result.Success) return NotFound(result.Message);
            return Ok(result.Data);
        }

        [HttpDelete("{clusterId}")]
        public async Task<IActionResult> Delete(string clusterId)
        {
            var result = await _clusterService.DeleteClusterAsync(clusterId);
            if (!result.Success) return NotFound(result.Message);
            return Ok(result.Data);
        }

        [HttpPost("{clusterId}/assign-driver/{driverId}")]
        public async Task<IActionResult> AssignDriver(string clusterId, string driverId)
        {
            var result = await _clusterService.AssignDriverAsync(clusterId, driverId);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Data);
        }

        [HttpGet("unassigned")]
        public async Task<IActionResult> GetUnassignedClusters()
        {
            var result = await _clusterService.GetUnassignedClustersAsync();
            if (!result.Success) return NotFound(result.Message);
            return Ok(result.Data);
        }
    }
}
