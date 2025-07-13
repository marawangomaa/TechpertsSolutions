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
            var customer = await customerService.GetCustomerByIdAsync(id);
            return Ok(new GeneralResponse<CustomerDTO>
            {
                Success = true,
                Message = "Customer retrieved successfully",
                Data = customer
            });
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CustomerEditDTO customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Invalid data provided",
                    Data = null
                });
            }

            var updatedCustomer = await customerService.UpdateCustomerAsync(id, customer);
            if (updatedCustomer == null)
            {
                return NotFound(new GeneralResponse<string>
                {
                    Success = false,
                    Message = $"Customer with ID {id} not found",
                    Data = null
                });
            }
            return Ok(new GeneralResponse<CustomerDTO>
            {
                Success = true,
                Message = "Customer updated successfully",
                Data = updatedCustomer
            });
        }

    }
}
