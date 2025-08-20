using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProfiles()
        {
            var response = await _profileService.GetAllProfilesAsync();
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfileById(string id)
        {
            var response = await _profileService.GetProfileByIdAsync(id);
            if (!response.Success)
                return NotFound(response);
            return Ok(response);
        }
        [HttpGet("customer/{userId}")]
        public async Task<IActionResult> GetCustomerProfile(string userId)
        {
            var result = await _profileService.GetCustomerRelatedInfoAsync(userId);

            if (!result.Success)
                return NotFound(new { result.Message });

            return Ok(result);
        }
        [HttpGet("techcompany/{userId}")]
        public async Task<IActionResult> GetTechCompanyProfile(string userId)
        {
            var result = await _profileService.GetTechCompanyRelatedInfoAsync(userId);

            if (!result.Success)
                return NotFound(new { result.Message });

            return Ok(result);
        }
        [HttpGet("deliveryperson/{userId}")]
        public async Task<IActionResult> GetDeliveryPersonProfile(string userId)
        {
            var result = await _profileService.GetDeliveryPersonRelatedInfoAsync(userId);

            if (!result.Success)
                return NotFound(new { result.Message });

            return Ok(result);
        }
    }
}
