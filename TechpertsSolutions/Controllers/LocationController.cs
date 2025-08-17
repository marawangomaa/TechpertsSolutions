using Core.DTOs;
using Core.DTOs.LocationDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace TechpertsSolutions.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocationController : ControllerBase
    {
        private readonly LocationService _locationService;

        public LocationController(LocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("distance")]
        public ActionResult<double> CalculateDistance(
            [FromQuery] double lat1,
            [FromQuery] double lon1,
            [FromQuery] double lat2,
            [FromQuery] double lon2)
        {
            var distance = _locationService.CalculateDistance(lat1, lon1, lat2, lon2);
            return Ok(distance);
        }

        [HttpGet("midpoint")]
        public ActionResult<(double Lat, double Lon)> GetMidpoint(
            [FromQuery] double lat1,
            [FromQuery] double lon1,
            [FromQuery] double lat2,
            [FromQuery] double lon2)
        {
            var midpoint = _locationService.GetMidpoint(lat1, lon1, lat2, lon2);
            return Ok(midpoint);
        }

        [HttpGet("company-to-customer")]
        public ActionResult<double> CompanyToCustomer(
            [FromQuery] double companyLat,
            [FromQuery] double companyLon,
            [FromQuery] double customerLat,
            [FromQuery] double customerLon)
        {
            var distance = _locationService.CompanyToCustomer(companyLat, companyLon, customerLat, customerLon);
            return Ok(distance);
        }

        [HttpGet("company-to-driver")]
        public ActionResult<double> CompanyToDriver(
            [FromQuery] double companyLat,
            [FromQuery] double companyLon,
            [FromQuery] double driverLat,
            [FromQuery] double driverLon)
        {
            var distance = _locationService.CompanyToDriver(companyLat, companyLon, driverLat, driverLon);
            return Ok(distance);
        }

        [HttpGet("driver-to-customer")]
        public ActionResult<double> DriverToCustomer(
            [FromQuery] double driverLat,
            [FromQuery] double driverLon,
            [FromQuery] double customerLat,
            [FromQuery] double customerLon)
        {
            var distance = _locationService.DriverToCustomer(driverLat, driverLon, customerLat, customerLon);
            return Ok(distance);
        }

        [HttpGet("driver-to-company")]
        public ActionResult<double> DriverToCompany(
            [FromQuery] double driverLat,
            [FromQuery] double driverLon,
            [FromQuery] double companyLat,
            [FromQuery] double companyLon)
        {
            var distance = _locationService.DriverToCompany(driverLat, driverLon, companyLat, companyLon);
            return Ok(distance);
        }
    }
}