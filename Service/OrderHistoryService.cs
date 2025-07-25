using Core.DTOs.OrderDTOs;
using TechpertsSolutions.Core.DTOs;
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

        public async Task<GeneralResponse<OrderHistoryReadDTO>> GetHistoryByIdAsync(string id)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<OrderHistoryReadDTO>
                {
                    Success = false,
                    Message = "Order History ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<OrderHistoryReadDTO>
                {
                    Success = false,
                    Message = "Invalid Order History ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                var history = await _historyRepo.GetFirstOrDefaultAsync(
                    h => h.Id == id,
                    includeProperties: "Orders,Orders.OrderItems,Orders.OrderItems.Product,Orders.Customer");
                
                if (history == null)
                {
                    return new GeneralResponse<OrderHistoryReadDTO>
                    {
                        Success = false,
                        Message = $"Order History with ID '{id}' not found.",
                        Data = null
                    };
                }
                
                return new GeneralResponse<OrderHistoryReadDTO>
                {
                    Success = true,
                    Message = "Order History retrieved successfully.",
                    Data = OrderHistoryMapper.MapToOrderHistoryReadDTO(history)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<OrderHistoryReadDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the order history.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<OrderHistoryReadDTO>> GetHistoryByCustomerIdAsync(string customerId)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return new GeneralResponse<OrderHistoryReadDTO>
                {
                    Success = false,
                    Message = "Customer ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(customerId, out _))
            {
                return new GeneralResponse<OrderHistoryReadDTO>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                var history = await _historyRepo.GetFirstOrDefaultAsync(
                    h => h.Orders.Any(o => o.CustomerId == customerId),
                    includeProperties: "Orders,Orders.OrderItems,Orders.OrderItems.Product,Orders.Customer");

                if (history == null)
                {
                    return new GeneralResponse<OrderHistoryReadDTO>
                    {
                        Success = false,
                        Message = $"Order History for Customer ID '{customerId}' not found.",
                        Data = null
                    };
                }

                return new GeneralResponse<OrderHistoryReadDTO>
                {
                    Success = true,
                    Message = "Order History retrieved successfully.",
                    Data = OrderHistoryMapper.MapToOrderHistoryReadDTO(history, customerId)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<OrderHistoryReadDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the order history.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<OrderHistoryReadDTO>>> GetAllOrderHistoriesAsync()
        {
            try
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
                
                return new GeneralResponse<IEnumerable<OrderHistoryReadDTO>>
                {
                    Success = true,
                    Message = "Order Histories retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<OrderHistoryReadDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving order histories.",
                    Data = null
                };
            }
        }
    }
}
