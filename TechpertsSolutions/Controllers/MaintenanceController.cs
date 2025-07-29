using Core.DTOs.MaintenanceDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceService _service;

        public MaintenanceController(IMaintenanceService service)
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
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "ID must not be null or empty.",
                    Data = "Invalid input"
                });
            }

            var response = await _service.GetByIdAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MaintenanceCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Validation failed: " + string.Join("; ", errors)
                });
            }

            var response = await _service.AddAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] MaintenanceUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Validation failed: " + string.Join("; ", errors)
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
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "ID must not be null or empty.",
                    Data = "Invalid input"
                });
            }

            var response = await _service.DeleteAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("tech-company/{techCompanyId}")]
        public async Task<IActionResult> GetByTechCompanyId(string techCompanyId)
        {
            if (string.IsNullOrWhiteSpace(techCompanyId))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Tech Company ID must not be null or empty.",
                    Data = "Invalid input"
                });
            }

            var response = await _service.GetByTechCompanyIdAsync(techCompanyId);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("available-requests")]
        public async Task<IActionResult> GetAvailableMaintenanceRequests()
        {
            var response = await _service.GetAvailableMaintenanceRequestsAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("{maintenanceId}/accept")]
        public async Task<IActionResult> AcceptMaintenanceRequest(string maintenanceId, [FromBody] string techCompanyId)
        {
            if (string.IsNullOrWhiteSpace(maintenanceId) || string.IsNullOrWhiteSpace(techCompanyId))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Maintenance ID and Tech Company ID must not be null or empty.",
                    Data = "Invalid input"
                });
            }

            var response = await _service.AcceptMaintenanceRequestAsync(maintenanceId, techCompanyId);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("{maintenanceId}/complete")]
        public async Task<IActionResult> CompleteMaintenance(string maintenanceId, [FromBody] CompleteMaintenanceRequest request)
        {
            if (string.IsNullOrWhiteSpace(maintenanceId))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Maintenance ID must not be null or empty.",
                    Data = "Invalid input"
                });
            }

            if (string.IsNullOrWhiteSpace(request.TechCompanyId))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Tech Company ID must not be null or empty.",
                    Data = "Invalid input"
                });
            }

            var response = await _service.CompleteMaintenanceAsync(maintenanceId, request.TechCompanyId, request.Notes);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("{maintenanceId}/status")]
        public async Task<IActionResult> UpdateMaintenanceStatus(string maintenanceId, [FromBody] string newStatus)
        {
            if (string.IsNullOrWhiteSpace(maintenanceId) || string.IsNullOrWhiteSpace(newStatus))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Maintenance ID and new status must not be null or empty.",
                    Data = "Invalid input"
                });
            }

            var response = await _service.UpdateMaintenanceStatusAsync(maintenanceId, newStatus);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }

    public class CompleteMaintenanceRequest
    {
        public string TechCompanyId { get; set; }
        public string Notes { get; set; }
    }
}
