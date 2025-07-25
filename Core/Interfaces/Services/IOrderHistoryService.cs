using TechpertsSolutions.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.OrderDTOs;

namespace Core.Interfaces.Services
{
    public interface IOrderHistoryService
    {
        Task<GeneralResponse<OrderHistoryReadDTO>> GetHistoryByIdAsync(string id);
        Task<GeneralResponse<OrderHistoryReadDTO>> GetHistoryByCustomerIdAsync(string customerId);
        Task<GeneralResponse<IEnumerable<OrderHistoryReadDTO>>> GetAllOrderHistoriesAsync();
    }
}
