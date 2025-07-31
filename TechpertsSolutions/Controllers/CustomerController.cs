using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;
using TechpertsSolutions.Core.DTOs.CustomerDTOs;
using Core.Interfaces.Services;
using Core.Interfaces;
using Core.DTOs;

namespace TechpertsSolutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService customerService;
        public CustomerController(ICustomerService _customerService) 
        {
            customerService = _customerService;
        }
        
        [HttpGet("All")]
        public async Task<IActionResult> GetAll() 
        {
            var response = await customerService.GetAllCustomersAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(string id) 
        {
            if (string.IsNullOrWhiteSpace(id)) 
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "ID must not be null or empty",
                    Data = "Invalid input"
                });
            }
            if (!Guid.TryParse(id, out Guid guidId)) 
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "ID format is invalid",
                    Data = "Expected GUID"
                });
            }
            
            var response = await customerService.GetCustomerByIdAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CustomerEditDTO customerDto)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "ID must not be null or empty.",
                    Data = "Invalid ID input."
                });
            }

            if (!Guid.TryParse(id, out _))
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid ID format. Expected GUID.",
                    Data = "Invalid ID format."
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Validation failed: " + string.Join("; ", errors),
                    Data = "Model validation failed."
                });
            }

            var response = await customerService.UpdateCustomerAsync(id, customerDto);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
