using Microsoft.AspNetCore.Mvc;
using TechpertsSolutions.Core.DTOs;
using TechpertsSolutions.Core.DTOs.Customer;
using Core.Interfaces;


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
            var customers = await customerService.GetAllCustomersAsync();
            return Ok(new GeneralResponse<List<CustomerDTO>> 
            {
                Success = true,
                Message = "All Customers retrieved successfully",
                Data = customers
            });
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
            try
            {
                var customer = await customerService.GetCustomerByIdAsync(id);
                return Ok(new GeneralResponse<CustomerDTO>
                {
                    Success = true,
                    Message = "Customer retrieved successfully",
                    Data = customer
                });
            }
            catch (Exception ex) 
            {
                return NotFound(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer not found",
                    Data = ex.Message
                });
            }
        }
        [HttpPatch("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CustomerEditDTO customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid data provided",
                    Data = "Not Valid Entry"
                });
            }

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
            try
            {
                var updatedCustomer = await customerService.UpdateCustomerAsync(id, customer);
                if (updatedCustomer == null)
                {
                    return NotFound(new GeneralResponse<string>
                    {
                        Success = false,
                        Message = $"Customer with ID {id} not found",
                        Data = "Not Updated"
                    });
                }
                return Ok(new GeneralResponse<CustomerDTO>
                {
                    Success = true,
                    Message = "Customer updated successfully",
                    Data = updatedCustomer
                });
            }
            catch (Exception ex)
            {
                return NotFound(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Customer not found",
                    Data = ex.Message
                });
            }
        }

    }
}
