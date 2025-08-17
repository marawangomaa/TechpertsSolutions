using Core.DTOs;
using Core.DTOs.OrderDTOs;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Interfaces.Services
{
    public interface IOrderService
    {
        Task<GeneralResponse<OrderReadDTO>> CreateOrderAsync(OrderCreateDTO dto);
        Task<GeneralResponse<OrderReadDTO>> GetOrderByIdAsync(string id);
        Task<GeneralResponse<IEnumerable<OrderReadDTO>>> GetAllOrdersAsync();
        Task<GeneralResponse<IEnumerable<OrderReadDTO>>> GetOrdersByCustomerIdAsync(string customerId);
        Task<GeneralResponse<IEnumerable<OrderHistoryReadDTO>>> GetOrderHistoryByCustomerIdAsync(string customerId);
        Task<GeneralResponse<bool>> UpdateOrderStatusAsync(string orderId, OrderStatus newStatus);
        Task<GeneralResponse<IEnumerable<OrderReadDTO>>> GetOrdersByStatusAsync(OrderStatus status);
    }
}
