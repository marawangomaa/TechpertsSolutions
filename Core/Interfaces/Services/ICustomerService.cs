using TechpertsSolutions.Core.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs.Customer;
using TechpertsSolutions.Core.Entities;

namespace Core.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<GeneralResponse<IEnumerable<CustomerDTO>>> GetAllCustomersAsync();
        Task<GeneralResponse<CustomerDTO>> GetCustomerByIdAsync(string id);
        Task<GeneralResponse<CustomerDTO>> GetCustomerByIdAsyn(string id);
        Task<GeneralResponse<CustomerEditDTO>> UpdateCustomerAsync(string id, CustomerEditDTO dto);
        Task<GeneralResponse<bool>> CleanupCustomerDataAsync(string userId);
    }
}
