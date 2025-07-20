using Core.DTOs.Orders;
using Core.Entities;
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
        private readonly IRepository<OrderHistory> _orderHistoryRepo;

        public OrderService(IRepository<Order> orderRepo, IRepository<OrderHistory> orderHistoryRepo)
        {
            _orderRepo = orderRepo;
            _orderHistoryRepo = orderHistoryRepo;
        }

        public async Task<OrderReadDTO> CreateOrderAsync(OrderCreateDTO dto)
        {
            var order = OrderMapper.ToEntity(dto);
            
            // Calculate total amount properly
            order.TotalAmount = order.OrderItems.Sum(i => i.ItemTotal);

            // Create or get order history for the customer
            var orderHistory = await GetOrCreateOrderHistoryAsync(dto.CustomerId);
            order.OrderHistoryId = orderHistory.Id;

            await _orderRepo.AddAsync(order);
            await _orderRepo.SaveChangesAsync();

            return OrderMapper.ToReadDTO(order);
        }

        public async Task<OrderReadDTO> GetOrderByIdAsync(string id)
        {
            var order = await _orderRepo.GetByIdWithIncludesAsync(id, 
                o => o.OrderItems, 
                o => o.Customer,
                o => o.OrderHistory);
            
            if (order == null)
            {
                return null;
            }
            
            return OrderMapper.ToReadDTO(order);
        }

        public async Task<IEnumerable<OrderReadDTO>> GetAllOrdersAsync()
        {
            var orders = await _orderRepo.GetAllWithIncludesAsync(
                o => o.OrderItems, 
                o => o.Customer,
                o => o.OrderHistory);
            return orders.Where(o => o != null).Select(OrderMapper.ToReadDTO).Where(dto => dto != null);
        }

        public async Task<IEnumerable<OrderReadDTO>> GetOrdersByCustomerIdAsync(string customerId)
        {
            var orders = await _orderRepo.FindWithIncludesAsync(
                o => o.CustomerId == customerId, 
                o => o.OrderItems, 
                o => o.Customer,
                o => o.OrderHistory);
            return orders.Where(o => o != null).Select(OrderMapper.ToReadDTO).Where(dto => dto != null);
        }

        private async Task<OrderHistory> GetOrCreateOrderHistoryAsync(string customerId)
        {
            // Try to find existing order history for the customer
            var existingHistory = await _orderHistoryRepo.GetFirstOrDefaultAsync(
                oh => oh.Orders.Any(o => o.CustomerId == customerId),
                includeProperties: "Orders");

            if (existingHistory != null)
            {
                return existingHistory;
            }

            // Create new order history if none exists
            var newHistory = new OrderHistory
            {
                Id = Guid.NewGuid().ToString(),
                Orders = new List<Order>()
            };

            await _orderHistoryRepo.AddAsync(newHistory);
            await _orderHistoryRepo.SaveChangesAsync();

            return newHistory;
        }
    }
}
