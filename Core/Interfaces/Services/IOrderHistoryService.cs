using Core.DTOs.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IOrderHistoryService
    {
        Task<OrderHistoryReadDTO> GetHistoryByIdAsync(string id);
        Task<OrderHistoryReadDTO> GetHistoryByCustomerIdAsync(string customerId);
        Task<IEnumerable<OrderHistoryReadDTO>> GetAllOrderHistoriesAsync();
    }
}
