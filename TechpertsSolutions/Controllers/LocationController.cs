using Core.DTOs.LocationDTOs;
using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechpertsSolutions.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpPost("nearest")]
        public async Task<ActionResult<GeneralResponse<IEnumerable<NearestTechCompanyDTO>>>> GetNearestTechCompanies([FromBody] LocationSearchDTO searchDto)
        {
            var result = await _locationService.GetNearestTechCompaniesAsync(searchDto);
            return Ok(result);
        }

        [HttpGet("nearest")]
        public async Task<ActionResult<GeneralResponse<NearestTechCompanyDTO>>> GetNearestTechCompany(
            [FromQuery] double latitude, 
            [FromQuery] double longitude, 
            [FromQuery] string? serviceType = null)
        {
            var result = await _locationService.GetNearestTechCompanyAsync(latitude, longitude, serviceType);
            return Ok(result);
        }

        [HttpGet("distance")]
        public async Task<ActionResult<GeneralResponse<double>>> CalculateDistance(
            [FromQuery] double lat1, 
            [FromQuery] double lon1, 
            [FromQuery] double lat2, 
            [FromQuery] double lon2)
        {
            var result = await _locationService.CalculateDistanceAsync(lat1, lon1, lat2, lon2);
            return Ok(result);
        }

        [HttpGet("geocode")]
        public async Task<ActionResult<GeneralResponse<LocationDTO>>> GetLocationFromAddress([FromQuery] string address)
        {
            var result = await _locationService.GetLocationFromAddressAsync(address);
            return Ok(result);
        }

        [HttpGet("reverse-geocode")]
        public async Task<ActionResult<GeneralResponse<LocationDTO>>> GetAddressFromCoordinates(
            [FromQuery] double latitude, 
            [FromQuery] double longitude)
        {
            var result = await _locationService.GetAddressFromCoordinatesAsync(latitude, longitude);
            return Ok(result);
        }

        [HttpPut("user/{userId}")]
        public async Task<ActionResult<GeneralResponse<bool>>> UpdateUserLocation(
            string userId, 
            [FromQuery] double latitude, 
            [FromQuery] double longitude)
        {
            var result = await _locationService.UpdateUserLocationAsync(userId, latitude, longitude);
            return Ok(result);
        }

        [HttpPut("techcompany/{techCompanyId}")]
        [Authorize(Roles = "TechCompany")]
        public async Task<ActionResult<GeneralResponse<bool>>> UpdateTechCompanyLocation(
            string techCompanyId, 
            [FromQuery] double latitude, 
            [FromQuery] double longitude, 
            [FromQuery] string address)
        {
            var result = await _locationService.UpdateTechCompanyLocationAsync(techCompanyId, latitude, longitude, address);
            return Ok(result);
        }

        [HttpGet("radius")]
        public async Task<ActionResult<GeneralResponse<IEnumerable<NearestTechCompanyDTO>>>> GetTechCompaniesInRadius(
            [FromQuery] double latitude, 
            [FromQuery] double longitude, 
            [FromQuery] double radiusInKm)
        {
            var result = await _locationService.GetTechCompaniesInRadiusAsync(latitude, longitude, radiusInKm);
            return Ok(result);
        }
    }
} 