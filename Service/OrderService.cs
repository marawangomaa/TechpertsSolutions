using Core.DTOs.Orders;
using Core.Interfaces;
using Core.Interfaces.Services;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepo;

        public OrderService(IRepository<Order> orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task<OrderReadDTO> CreateOrderAsync(OrderCreateDTO dto)
        {
            var order = OrderMapper.ToEntity(dto);
            order.TotalAmount = order.OrderItems.Sum(i => i.ItemTotal);

            await _orderRepo.AddAsync(order);
            await _orderRepo.SaveChangesAsync();

            return OrderMapper.ToReadDTO(order);
        }

        public async Task<OrderReadDTO> GetOrderByIdAsync(string id)
        {
            var order = await _orderRepo.GetByIdWithIncludesAsync(id, o => o.OrderItems, o => o.OrderItems.Select(i => i.Product));
            return OrderMapper.ToReadDTO(order!);
        }

        public async Task<IEnumerable<OrderReadDTO>> GetAllOrdersAsync()
        {
            var orders = await _orderRepo.GetAllWithIncludesAsync(o => o.OrderItems, o => o.OrderItems.Select(i => i.Product));
            return orders.Select(OrderMapper.ToReadDTO);
        }

        public async Task<IEnumerable<OrderReadDTO>> GetOrdersByCustomerIdAsync(string customerId)
        {
            var orders = await _orderRepo.FindWithIncludesAsync(o => o.CustomerId == customerId, o => o.OrderItems, o => o.OrderItems.Select(i => i.Product));
            return orders.Select(OrderMapper.ToReadDTO);
        }
    }
}
