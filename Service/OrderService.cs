using Core.DTOs.OrderDTOs;
using TechpertsSolutions.Core.DTOs;
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

        public async Task<GeneralResponse<OrderReadDTO>> CreateOrderAsync(OrderCreateDTO dto)
        {
            // Input validation
            if (dto == null)
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "Order data cannot be null.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(dto.CustomerId))
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "Customer ID is required.",
                    Data = null
                };
            }

            if (!Guid.TryParse(dto.CustomerId, out _))
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Expected GUID format.",
                    Data = null
                };
            }

            if (dto.OrderItems == null || !dto.OrderItems.Any())
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "Order must contain at least one item.",
                    Data = null
                };
            }

            try
            {
                var order = OrderMapper.ToEntity(dto);
                
                // Calculate total amount properly
                order.TotalAmount = order.OrderItems.Sum(i => i.ItemTotal);

                // Create or get order history for the customer
                var orderHistory = await GetOrCreateOrderHistoryAsync(dto.CustomerId);
                order.OrderHistoryId = orderHistory.Id;

                await _orderRepo.AddAsync(order);
                await _orderRepo.SaveChangesAsync();

                return new GeneralResponse<OrderReadDTO>
                {
                    Success = true,
                    Message = "Order created successfully.",
                    Data = OrderMapper.ToReadDTO(order)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while creating the order.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<OrderReadDTO>> GetOrderByIdAsync(string id)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "Order ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "Invalid Order ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                var order = await _orderRepo.GetByIdWithIncludesAsync(id, 
                    o => o.OrderItems, 
                    o => o.Customer,
                    o => o.OrderHistory);
                
                if (order == null)
                {
                    return new GeneralResponse<OrderReadDTO>
                    {
                        Success = false,
                        Message = $"Order with ID '{id}' not found.",
                        Data = null
                    };
                }
                
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = true,
                    Message = "Order retrieved successfully.",
                    Data = OrderMapper.ToReadDTO(order)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the order.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<OrderReadDTO>>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _orderRepo.GetAllWithIncludesAsync(
                    o => o.OrderItems, 
                    o => o.Customer,
                    o => o.OrderHistory);
                
                var orderDtos = orders.Where(o => o != null).Select(OrderMapper.ToReadDTO).Where(dto => dto != null);
                
                return new GeneralResponse<IEnumerable<OrderReadDTO>>
                {
                    Success = true,
                    Message = "Orders retrieved successfully.",
                    Data = orderDtos
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<OrderReadDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving orders.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<OrderReadDTO>>> GetOrdersByCustomerIdAsync(string customerId)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return new GeneralResponse<IEnumerable<OrderReadDTO>>
                {
                    Success = false,
                    Message = "Customer ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(customerId, out _))
            {
                return new GeneralResponse<IEnumerable<OrderReadDTO>>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                var orders = await _orderRepo.FindWithIncludesAsync(
                    o => o.CustomerId == customerId, 
                    o => o.OrderItems, 
                    o => o.Customer,
                    o => o.OrderHistory);
                
                var orderDtos = orders.Where(o => o != null).Select(OrderMapper.ToReadDTO).Where(dto => dto != null);
                
                return new GeneralResponse<IEnumerable<OrderReadDTO>>
                {
                    Success = true,
                    Message = "Customer orders retrieved successfully.",
                    Data = orderDtos
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<OrderReadDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving customer orders.",
                    Data = null
                };
            }
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
