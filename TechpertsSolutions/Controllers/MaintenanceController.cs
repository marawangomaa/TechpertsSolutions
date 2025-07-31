using Core.DTOs.MaintenanceDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;
using Core.Enums;
using Core.Utilities;
using Core.DTOs;
using Microsoft.AspNetCore.Authorization;

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] MaintenanceCreateDTO dto, [FromQuery] MaintenanceStatus status = MaintenanceStatus.Requested)
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

            // Create a new DTO with the status from query parameter
            var dtoWithStatus = new MaintenanceCreateDTO
            {
                CustomerId = dto.CustomerId,
                TechCompanyId = dto.TechCompanyId,
                WarrantyId = dto.WarrantyId,
                Status = status,
                Notes = dto.Notes
            };

            var response = await _service.AddAsync(dtoWithStatus);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(string id, [FromBody] MaintenanceUpdateDTO dto, [FromQuery] MaintenanceStatus? status = null)
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

            // Create a new DTO with the status from query parameter if provided
            var dtoWithStatus = new MaintenanceUpdateDTO
            {
                CustomerId = dto.CustomerId,
                TechCompanyId = dto.TechCompanyId,
                WarrantyId = dto.WarrantyId,
                Status = status ?? dto.Status,
                Notes = dto.Notes,
                CompletedDate = dto.CompletedDate
            };

            var response = await _service.UpdateAsync(id, dtoWithStatus);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        public async Task<IActionResult> UpdateMaintenanceStatus(string maintenanceId, [FromQuery] MaintenanceStatus status, [FromQuery] string? notes = null)
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

            var response = await _service.UpdateMaintenanceStatusAsync(maintenanceId, status.GetStringValue());
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("nearest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNearestMaintenance([FromQuery] string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId) || !Guid.TryParse(customerId, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid customer ID.",
                    Data = customerId
                });
            }

            var response = await _service.GetNearestMaintenanceAsync(customerId);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }

    public class CompleteMaintenanceRequest
    {
        public string TechCompanyId { get; set; } = string.Empty;
        
        public string Notes { get; set; } = string.Empty;
    }
}
