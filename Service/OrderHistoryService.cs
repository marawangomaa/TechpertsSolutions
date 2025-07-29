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
        private readonly IRepository<Order> _orderRepo;

        public OrderHistoryService(IRepository<OrderHistory> historyRepo, IRepository<Order> orderRepo)
        {
            _historyRepo = historyRepo;
            _orderRepo = orderRepo;
        }

        public async Task<GeneralResponse<OrderHistoryReadDTO>> GetHistoryByIdAsync(string id)
        {
            
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
                    includeProperties: "Orders,Orders.OrderItems,Orders.OrderItems.Product,Orders.Customer,Orders.Customer.User");
                
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
                // Get all order histories that contain orders for this customer
                var histories = await _historyRepo.FindWithStringIncludesAsync(
                    h => h.Orders.Any(o => o.CustomerId == customerId),
                    includeProperties: "Orders,Orders.OrderItems,Orders.OrderItems.Product,Orders.Customer,Orders.Customer.User");

                if (!histories.Any())
                {
                    // Return empty order history if no orders found for customer
                    return new GeneralResponse<OrderHistoryReadDTO>
                    {
                        Success = true,
                        Message = $"No orders found for Customer ID '{customerId}'.",
                        Data = OrderHistoryMapper.CreateEmptyOrderHistoryReadDTO()
                    };
                }

                // Combine all orders from all histories for this customer
                var allOrders = histories
                    .SelectMany(h => h.Orders ?? new List<Order>())
                    .Where(o => o.CustomerId == customerId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

                // Create a virtual order history with all customer orders
                var virtualHistory = new OrderHistory
                {
                    Id = $"customer-{customerId}",
                    Orders = allOrders
                };

                return new GeneralResponse<OrderHistoryReadDTO>
                {
                    Success = true,
                    Message = $"Order History retrieved successfully. Found {allOrders.Count} orders.",
                    Data = OrderHistoryMapper.MapToOrderHistoryReadDTO(virtualHistory)
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
                // Get all order histories with proper includes in a single query
                var histories = await _historyRepo.FindWithStringIncludesAsync(
                    h => true, // Get all histories
                    includeProperties: "Orders,Orders.OrderItems,Orders.OrderItems.Product,Orders.Customer,Orders.Customer.User");
                
                var result = histories
                    .Where(h => h != null)
                    .Select(OrderHistoryMapper.MapToOrderHistoryReadDTO)
                    .Where(dto => dto != null)
                    .ToList();
                
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

        public async Task<GeneralResponse<IEnumerable<OrderReadDTO>>> GetCustomerOrdersAsync(string customerId)
        {
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
                // Get all order histories that contain orders for this customer
                var histories = await _historyRepo.FindWithStringIncludesAsync(
                    h => h.Orders.Any(o => o.CustomerId == customerId),
                    includeProperties: "Orders,Orders.OrderItems,Orders.OrderItems.Product,Orders.Customer,Orders.Customer.User");

                // Extract all orders for this customer from all histories
                var customerOrders = histories
                    .SelectMany(h => h.Orders ?? new List<Order>())
                    .Where(o => o.CustomerId == customerId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

                var orderDtos = customerOrders
                    .Where(o => o != null)
                    .Select(OrderMapper.ToReadDTO)
                    .Where(dto => dto != null)
                    .ToList();

                return new GeneralResponse<IEnumerable<OrderReadDTO>>
                {
                    Success = true,
                    Message = $"Customer orders retrieved successfully. Found {orderDtos.Count} orders.",
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

        public async Task<GeneralResponse<OrderHistoryReadDTO>> GetHistoryByOrderIdAsync(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
            {
                return new GeneralResponse<OrderHistoryReadDTO>
                {
                    Success = false,
                    Message = "Order ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(orderId, out _))
            {
                return new GeneralResponse<OrderHistoryReadDTO>
                {
                    Success = false,
                    Message = "Invalid Order ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                // Find the order history that contains this specific order
                var history = await _historyRepo.GetFirstOrDefaultAsync(
                    h => h.Orders.Any(o => o.Id == orderId),
                    includeProperties: "Orders,Orders.OrderItems,Orders.OrderItems.Product,Orders.Customer,Orders.Customer.User");

                if (history == null)
                {
                    return new GeneralResponse<OrderHistoryReadDTO>
                    {
                        Success = false,
                        Message = $"Order History for Order ID '{orderId}' not found.",
                        Data = null
                    };
                }

                // Filter to only include the specific order
                var filteredHistory = new OrderHistory
                {
                    Id = history.Id,
                    Orders = history.Orders?.Where(o => o.Id == orderId).ToList() ?? new List<Order>()
                };

                return new GeneralResponse<OrderHistoryReadDTO>
                {
                    Success = true,
                    Message = "Order History retrieved successfully.",
                    Data = OrderHistoryMapper.MapToOrderHistoryReadDTO(filteredHistory)
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

        public async Task<GeneralResponse<IEnumerable<OrderReadDTO>>> GetCustomerOrdersDirectAsync(string customerId)
        {
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
                // Get orders directly from the Order repository
                var orders = await _orderRepo.FindWithStringIncludesAsync(
                    o => o.CustomerId == customerId,
                    includeProperties: "OrderItems,OrderItems.Product,Customer,Customer.User,OrderHistory");

                var orderDtos = orders
                    .Where(o => o != null)
                    .Select(OrderMapper.ToReadDTO)
                    .Where(dto => dto != null)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

                return new GeneralResponse<IEnumerable<OrderReadDTO>>
                {
                    Success = true,
                    Message = $"Customer orders retrieved successfully. Found {orderDtos.Count} orders.",
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
    }
}
