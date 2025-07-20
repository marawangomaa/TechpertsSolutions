using Core.DTOs.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderReadDTO> CreateOrderAsync(OrderCreateDTO dto);
        Task<OrderReadDTO> GetOrderByIdAsync(string id);
        Task<IEnumerable<OrderReadDTO>> GetAllOrdersAsync();
        Task<IEnumerable<OrderReadDTO>> GetOrdersByCustomerIdAsync(string customerId);
    }
}
