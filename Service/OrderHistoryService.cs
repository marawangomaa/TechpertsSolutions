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
    public class OrderHistoryService : IOrderHistoryService
    {
        private readonly IRepository<OrderHistory> _historyRepo;

        public OrderHistoryService(IRepository<OrderHistory> historyRepo)
        {
            _historyRepo = historyRepo;
        }

        public async Task<OrderHistoryReadDTO> GetHistoryByIdAsync(string id)
        {
            var history = await _historyRepo.GetFirstOrDefaultAsync(
                h => h.Id == id,
                includeProperties: "Orders,Orders.OrderItems,Orders.OrderItems.Product,Orders.Customer");
            
            return OrderHistoryMapper.MapToOrderHistoryReadDTO(history);
        }

        public async Task<OrderHistoryReadDTO> GetHistoryByCustomerIdAsync(string customerId)
        {
            var history = await _historyRepo.GetFirstOrDefaultAsync(
                h => h.Orders.Any(o => o.CustomerId == customerId),
                includeProperties: "Orders,Orders.OrderItems,Orders.OrderItems.Product,Orders.Customer");

            return OrderHistoryMapper.MapToOrderHistoryReadDTO(history, customerId);
        }

        public async Task<IEnumerable<OrderHistoryReadDTO>> GetAllOrderHistoriesAsync()
        {
            var histories = await _historyRepo.GetAllAsync();
            
            var result = new List<OrderHistoryReadDTO>();
            
            foreach (var history in histories)
            {
                if (history != null)
                {
                    // Load related data for each history
                    var historyWithIncludes = await _historyRepo.GetFirstOrDefaultAsync(
                        h => h.Id == history.Id,
                        includeProperties: "Orders,Orders.OrderItems,Orders.OrderItems.Product,Orders.Customer");
                    
                    result.Add(OrderHistoryMapper.MapToOrderHistoryReadDTO(historyWithIncludes));
                }
            }
            
            return result;
        }
    }
}
