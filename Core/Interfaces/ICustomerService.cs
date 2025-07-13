using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs.Customer;

namespace Core.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync();
        Task<CustomerDTO?> GetCustomerByIdAsync(string id);
        Task<CustomerEditDTO?> UpdateCustomerAsync(string id, CustomerEditDTO dto);
        Task CleanupCustomerDataAsync(string userId);
    }
}
