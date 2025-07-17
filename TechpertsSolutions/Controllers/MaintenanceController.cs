using Core.DTOs.Maintenance;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class MaintenanceController : ControllerBase
    //{
    //    private readonly IMaintenanceService _service;

    //    public MaintenanceController(IMaintenanceService service)
    //    {
    //        _service = service;
    //    }

    //    [HttpGet]
    //    public async Task<IActionResult> GetAll()
    //    {
    //        var maintenances = await _service.GetAllAsync();
    //        return Ok(new GeneralResponse<IEnumerable<MaintenanceDTO>>
    //        {
    //            Success = true,
    //            Message = "All maintenances fetched",
    //            Data = maintenances
    //        });
    //    }

    //    [HttpGet("{id}")]
    //    public async Task<IActionResult> GetById(string id)
    //    {
    //        if (string.IsNullOrWhiteSpace(id))
    //        {
    //            return BadRequest(new GeneralResponse<string>
    //            {
    //                Success = false,
    //                Message = "ID must not be null or empty",
    //                Data = "Invalid input"
    //            });
    //        }

    //        var maintenance = await _service.GetByIdAsync(id);
    //        if (maintenance == null)
    //        {
    //            return NotFound(new GeneralResponse<string>
    //            {
    //                Success = false,
    //                Message = "Maintenance not found",
    //                Data = id
    //            });
    //        }

    //        return Ok(new GeneralResponse<MaintenanceDetailsDTO>
    //        {
    //            Success = true,
    //            Message = "Maintenance details fetched",
    //            Data = maintenance
    //        });
    //    }

    //    [HttpPost]
    //    public async Task<IActionResult> Create()
    //    {
    //        try
    //        {
    //            var created = await _service.AddAsync();
    //            return Ok(new GeneralResponse<MaintenanceDTO>
    //            {
    //                Success = true,
    //                Message = "Maintenance created successfully",
    //                Data = created
    //            });
    //        }
    //        catch (Exception ex)
    //        {
    //            return StatusCode(500, new GeneralResponse<string>
    //            {
    //                Success = false,
    //                Message = "Error while creating maintenance",
    //                Data = ex.Message
    //            });
    //        }
    //    }

    //    [HttpPut("{id}")]
    //    public async Task<IActionResult> Update(string id, [FromBody] MaintenanceUpdateDTO dto)
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            return BadRequest(new GeneralResponse<string>
    //            {
    //                Success = false,
    //                Message = "Invalid data provided",
    //                Data = "Invalid ModelState"
    //            });
    //        }

    //        try
    //        {
    //            var updated = await _service.UpdateAsync(id, dto);
    //            if (!updated)
    //            {
    //                return NotFound(new GeneralResponse<string>
    //                {
    //                    Success = false,
    //                    Message = $"Maintenance with ID {id} not found or invalid related entities",
    //                    Data = id
    //                });
    //            }

    //            return Ok(new GeneralResponse<string>
    //            {
    //                Success = true,
    //                Message = "Maintenance updated successfully",
    //                Data = id
    //            });
    //        }
    //        catch (Exception ex)
    //        {
    //            return StatusCode(500, new GeneralResponse<string>
    //            {
    //                Success = false,
    //                Message = "Error while updating maintenance",
    //                Data = ex.Message
    //            });
    //        }
    //    }

    //    [HttpDelete("{id}")]
    //    public async Task<IActionResult> Delete(string id)
    //    {
    //        if (string.IsNullOrWhiteSpace(id))
    //        {
    //            return BadRequest(new GeneralResponse<string>
    //            {
    //                Success = false,
    //                Message = "ID must not be null or empty",
    //                Data = "Invalid input"
    //            });
    //        }

    //        var deleted = await _service.DeleteAsync(id);
    //        if (!deleted)
    //        {
    //            return NotFound(new GeneralResponse<string>
    //            {
    //                Success = false,
    //                Message = $"Maintenance with ID {id} not found",
    //                Data = id
    //            });
    //        }

    //        return Ok(new GeneralResponse<string>
    //        {
    //            Success = true,
    //            Message = "Maintenance deleted successfully",
    //            Data = id
    //        });
    //    }
    //}
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
            var maintenances = await _service.GetAllAsync();

            return Ok(new GeneralResponse<IEnumerable<MaintenanceDTO>>
            {
                Success = true,
                Message = "All maintenances fetched successfully.",
                Data = maintenances
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
                    Message = "ID must not be null or empty.",
                    Data = "Invalid input"
                });
            }

            var maintenance = await _service.GetByIdAsync(id);
            if (maintenance == null)
            {
                return NotFound(new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"Maintenance with ID '{id}' not found.",
                    Data = id
                });
            }

            return Ok(new GeneralResponse<MaintenanceDetailsDTO>
            {
                Success = true,
                Message = "Maintenance details fetched successfully.",
                Data = maintenance
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(MaintenanceCreateDTO dto)
        {
            try
            {
                var created = await _service.AddAsync(dto);

                return Ok(new GeneralResponse<MaintenanceDTO>
                {
                    Success = true,
                    Message = "Maintenance created successfully.",
                    Data = created
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while creating maintenance.",
                    Data = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] MaintenanceUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid data provided.",
                    Data = "Model validation failed"
                });
            }

            try
            {
                var updated = await _service.UpdateAsync(id, dto);

                if (!updated)
                {
                    return NotFound(new GeneralResponse<string>
                    {
                        Success = false,
                        Message = $"Maintenance with ID '{id}' not found or related entity is invalid.",
                        Data = id
                    });
                }

                return Ok(new GeneralResponse<string>
                {
                    Success = true,
                    Message = "Maintenance updated successfully.",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while updating maintenance.",
                    Data = ex.Message
                });
            }
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

            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"Maintenance with ID '{id}' not found.",
                    Data = id
                });
            }

            return Ok(new GeneralResponse<string>
            {
                Success = true,
                Message = "Maintenance deleted successfully.",
                Data = id
            });
        }
    }
}
