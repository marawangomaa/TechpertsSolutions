using Core.DTOs.OrderDTOs;
using Core.DTOs;
using TechpertsSolutions.Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Utilities;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<OrderHistory> _orderHistoryRepo;
        private readonly INotificationService _notificationService;

        public OrderService(IRepository<Order> orderRepo, IRepository<OrderHistory> orderHistoryRepo, INotificationService notificationService)
        {
            _orderRepo = orderRepo;
            _orderHistoryRepo = orderHistoryRepo;
            _notificationService = notificationService;
        }

        public async Task<GeneralResponse<OrderReadDTO>> CreateOrderAsync(OrderCreateDTO dto)
        {
            
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
                
                // Calculate total amount
                order.TotalAmount = order.OrderItems.Sum(i => i.ItemTotal);

                // Get or create order history for this customer
                var orderHistory = await GetOrCreateOrderHistoryAsync(dto.CustomerId);
                order.OrderHistoryId = orderHistory.Id;

                await _orderRepo.AddAsync(order);
                await _orderRepo.SaveChangesAsync();

                // Send notification to admin about new order
                await _notificationService.SendNotificationToRoleAsync(
                    "Admin",
                    $"New order #{order.Id} has been created by customer {order.CustomerId}",
                    NotificationType.OrderCreated,
                    order.Id,
                    "Order"
                );

                // Get the created order with all includes to return proper data
                var createdOrder = await _orderRepo.GetFirstOrDefaultAsync(
                    o => o.Id == order.Id,
                    includeProperties: "OrderItems,OrderItems.Product,Customer,Customer.User,OrderHistory");

                return new GeneralResponse<OrderReadDTO>
                {
                    Success = true,
                    Message = "Order created successfully.",
                    Data = OrderMapper.ToReadDTO(createdOrder)
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
                // Comprehensive includes for detailed order view
                var order = await _orderRepo.GetByIdWithIncludesAsync(id,
                    o => o.OrderItems,
                    o => o.Customer,
                    o => o.OrderHistory,
                    o => o.Delivery,
                    o => o.ServiceUsage);

                if (order == null)
                {
                    return new GeneralResponse<OrderReadDTO>
                    {
                        Success = false,
                        Message = $"Order with ID '{id}' not found.",
                        Data = null
                    };
                }

                // Get order items with their products using string includes for nested properties
                var orderWithItems = await _orderRepo.FindWithStringIncludesAsync(
                    o => o.Id == id,
                    includeProperties: "OrderItems,OrderItems.Product,OrderItems.Product.Category,OrderItems.Product.SubCategory,OrderItems.Product.TechCompany,Customer,Customer.User,OrderHistory,Delivery,ServiceUsage");

                var orderEntity = orderWithItems.FirstOrDefault();
                if (orderEntity == null)
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
                    Data = OrderMapper.ToReadDTO(orderEntity)
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
                // Optimized includes for order listing with all necessary related data
                var allOrders = await _orderRepo.FindWithStringIncludesAsync(
                    o => true, // This will match all orders
                    includeProperties: "OrderItems,OrderItems.Product,OrderItems.Product.Category,OrderItems.Product.SubCategory,OrderItems.Product.TechCompany,Customer,Customer.User,OrderHistory,Delivery,ServiceUsage");
                
                var orderDtos = allOrders.Where(o => o != null).Select(OrderMapper.ToReadDTO).Where(dto => dto != null);
                
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
                // Optimized includes for customer orders with all necessary related data
                var orders = await _orderRepo.FindWithStringIncludesAsync(
                    o => o.CustomerId == customerId, 
                    includeProperties: "OrderItems,OrderItems.Product,OrderItems.Product.Category,OrderItems.Product.SubCategory,OrderItems.Product.TechCompany,Customer,Customer.User,OrderHistory,Delivery,ServiceUsage");
                
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
            // First, try to find an existing OrderHistory that has orders for this customer
            var existingHistory = await _orderHistoryRepo.GetFirstOrDefaultAsync(
                oh => oh.Orders.Any(o => o.CustomerId == customerId),
                includeProperties: "Orders");

            if (existingHistory != null)
            {
                return existingHistory;
            }

            // If no existing history found, create a new one
            var newHistory = new OrderHistory
            {
                Id = Guid.NewGuid().ToString(),
                Orders = new List<Order>()
            };

            await _orderHistoryRepo.AddAsync(newHistory);
            await _orderHistoryRepo.SaveChangesAsync();

            return newHistory;
        }

        public async Task<GeneralResponse<IEnumerable<OrderHistoryReadDTO>>> GetOrderHistoryByCustomerIdAsync(string customerId)
        {
            
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return new GeneralResponse<IEnumerable<OrderHistoryReadDTO>>
                {
                    Success = false,
                    Message = "Customer ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(customerId, out _))
            {
                return new GeneralResponse<IEnumerable<OrderHistoryReadDTO>>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                var orderHistories = await _orderHistoryRepo.FindWithStringIncludesAsync(
                    oh => oh.Orders.Any(o => o.CustomerId == customerId),
                    includeProperties: "Orders,Orders.OrderItems,Orders.OrderItems.Product,Orders.Customer,Orders.Customer.User");

                var orderHistoryDtos = orderHistories
                    .Where(oh => oh != null)
                    .Select(OrderHistoryMapper.MapToOrderHistoryReadDTO)
                    .Where(dto => dto != null);

                return new GeneralResponse<IEnumerable<OrderHistoryReadDTO>>
                {
                    Success = true,
                    Message = "Customer order history retrieved successfully.",
                    Data = orderHistoryDtos
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<OrderHistoryReadDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving customer order history.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<bool>> UpdateOrderStatusAsync(string orderId, OrderStatus newStatus)
        {
            
            if (string.IsNullOrWhiteSpace(orderId))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Order ID cannot be null or empty.",
                    Data = false
                };
            }

            if (!Guid.TryParse(orderId, out _))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Invalid Order ID format. Expected GUID format.",
                    Data = false
                };
            }

            try
            {
                var order = await _orderRepo.GetByIdAsync(orderId);
                if (order == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Order not found.",
                        Data = false
                    };
                }

                order.Status = newStatus;
                _orderRepo.Update(order);
                await _orderRepo.SaveChangesAsync();

                // Send notification to customer about order status change
                await _notificationService.SendNotificationAsync(
                    order.CustomerId,
                    $"Your order #{order.Id} status has been updated to '{newStatus.GetStringValue()}'",
                    NotificationType.OrderStatusChanged,
                    order.Id,
                    "Order"
                );

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = $"Order status updated successfully to '{newStatus.GetStringValue()}'.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating order status.",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<OrderReadDTO>>> GetOrdersByStatusAsync(OrderStatus status)
        {
            try
            {
                var orders = await _orderRepo.FindWithStringIncludesAsync(
                    o => o.Status == status,
                    includeProperties: "OrderItems,OrderItems.Product,Customer,Customer.User,OrderHistory");

                var orderDtos = orders
                    .Where(o => o != null)
                    .Select(OrderMapper.ToReadDTO)
                    .Where(dto => dto != null);

                return new GeneralResponse<IEnumerable<OrderReadDTO>>
                {
                    Success = true,
                    Message = $"Orders with status '{status.GetStringValue()}' retrieved successfully.",
                    Data = orderDtos
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<OrderReadDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving orders by status.",
                    Data = null
                };
            }
        }
    }
}
