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
            var history = await _historyRepo.GetByIdWithIncludesAsync(id, h => h.Orders!);
            return new OrderHistoryReadDTO
            {
                Id = history!.Id,
                Orders = history.Orders.Select(OrderMapper.ToReadDTO).ToList()
            };
        }
    }
}
