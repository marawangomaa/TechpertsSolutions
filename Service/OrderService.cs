using Core.DTOs;
using Core.DTOs.DeliveryDTOs;
using Core.DTOs.OrderDTOs;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.Utilities;
using Stripe;
using Stripe.TestHelpers;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data;

namespace Service
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<Delivery> _deliveryRepo;
        private readonly IRepository<DeliveryPerson> _deliveryPersonRepo;
        private readonly IRepository<DeliveryOffer> _deliveryOfferRepo;
        private readonly IRepository<OrderHistory> _orderHistoryRepo;
        private readonly TechpertsContext _dbContext;
        private readonly ICustomerService _customerService;
        private readonly IDeliveryService _deliveryService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IRepository<Order> orderRepo,
            IRepository<OrderHistory> orderHistoryRepo,
            IRepository<Delivery> deliveryRepo,
            IRepository<DeliveryPerson> deliveryPersonRepo,
            IRepository<DeliveryOffer> deliveryOfferRepo,
            INotificationService notificationService,
            IDeliveryService deliveryService,
            ICustomerService customerService,
            ILogger<OrderService> logger,
            TechpertsContext dbContext
        )
        {
            _customerService = customerService;
            _orderRepo = orderRepo;
            _orderHistoryRepo = orderHistoryRepo;
            _deliveryRepo = deliveryRepo;
            _deliveryPersonRepo = deliveryPersonRepo;
            _deliveryOfferRepo = deliveryOfferRepo;
            _deliveryService = deliveryService;
            _notificationService = notificationService;
            _dbContext = dbContext;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GeneralResponse<OrderReadDTO>> CreateOrderAsync(OrderCreateDTO dto)
        {
            if (dto == null)
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "Order data cannot be null.",
                    Data = null,
                };

            if (string.IsNullOrWhiteSpace(dto.CustomerId))
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "Customer ID is required.",
                    Data = null,
                };

            if (!Guid.TryParse(dto.CustomerId, out _))
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Expected GUID format.",
                    Data = null,
                };

            if (dto.OrderItems == null || !dto.OrderItems.Any())
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "Order must contain at least one item.",
                    Data = null,
                };

            try
            {
                var order = OrderMapper.ToEntity(dto);
                order.TotalAmount = order.OrderItems.Sum(i => i.ItemTotal);

                var orderHistory = await GetOrCreateOrderHistoryAsync(dto.CustomerId);
                order.OrderHistoryId = orderHistory.Id;

                await _orderRepo.AddAsync(order);
                await _orderRepo.SaveChangesAsync();

                DeliveryCreateDTO? deliveryDto = null;
                GeneralResponse<DeliveryReadDTO>? deliveryResponse = null;

                double? latitude = dto.DeliveryLatitude;
                double? longitude = dto.DeliveryLongitude;

                if (!latitude.HasValue || !longitude.HasValue)
                {
                    var customer = await _customerService.GetCustomerByIdAsync(dto.CustomerId);
                    if (customer != null)
                    {
                        latitude = customer.Data.Latitude;
                        longitude = customer.Data.Longitude;
                    }
                }

                if (latitude.HasValue && longitude.HasValue)
                {
                    deliveryDto = new DeliveryCreateDTO
                    {
                        OrderId = order.Id,
                        CustomerLatitude = latitude.Value,
                        CustomerLongitude = longitude.Value,
                    };

                    deliveryResponse = await _deliveryService.CreateAsync(deliveryDto);
                }
                else
                {
                    _logger.LogWarning("No delivery coordinates provided for order {OrderId}. Delivery creation skipped.", order.Id);
                }


                var createdOrder = await _orderRepo.GetFirstOrDefaultAsync(
                        o => o.Id == order.Id,
                        query => query.Include(o => o.OrderItems)
                                    .ThenInclude(oi => oi.Product)
                                        .ThenInclude(p => p.TechCompany)
                                .Include(o => o.Customer)
                                    .ThenInclude(c => c.User)
                                .Include(o => o.OrderHistory)
                                .Include(o => o.Deliveries)
                                    .ThenInclude(d => d.DeliveryPerson)
                                        .ThenInclude(dp => dp.User)
                    );

                await _notificationService.SendNotificationToRoleAsync(
                    "Admin",
                    NotificationType.OrderCreated,
                    order.Id,
                    "Order",
                    order.Id
                );

                await _notificationService.SendNotificationAsync(
                    createdOrder.Customer.UserId,
                    NotificationType.OrderCreated,
                    order.Id,
                    "Order",
                    order.Id
                );

                var techCompanyIds = createdOrder
                    .OrderItems.Select(oi => oi.Product.TechCompany.UserId)
                    .Distinct()
                    .ToList();

                await _notificationService.SendNotificationsToMultipleUsers(
                    techCompanyIds,
                    NotificationType.OrderCreated,
                    order.Id,
                    "Order",
                    order.Id
                );

                return new GeneralResponse<OrderReadDTO>
                {
                    Success = true,
                    Message =
                        "Order created successfully, notifications sent, and delivery offers sent to nearby drivers.",
                    Data = OrderMapper.ToReadDTO(createdOrder),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateOrderAsync: Failed to create order.");
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message =
                        $"An unexpected error occurred while creating the order. {ex.Message}",
                    Data = null,
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
                    Data = null,
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "Invalid Order ID format. Expected GUID format.",
                    Data = null,
                };
            }

            try
            {
                var order = await _orderRepo.GetByIdWithIncludesAsync(
                    id,
                    o => o.OrderItems,
                    o => o.Customer,
                    o => o.OrderHistory,
                    o => o.Deliveries,
                    o => o.ServiceUsage
                );

                if (order == null)
                {
                    return new GeneralResponse<OrderReadDTO>
                    {
                        Success = false,
                        Message = $"Order with ID '{id}' not found.",
                        Data = null,
                    };
                }

                var orderWithItems = await _orderRepo.FindWithStringIncludesAsync(
                    o => o.Id == id,
                    includeProperties: "OrderItems,OrderItems.Product,OrderItems.Product.Category,OrderItems.Product.SubCategory,OrderItems.Product.TechCompany,Customer,Customer.User,OrderHistory,Delivery,ServiceUsage"
                );

                var orderEntity = orderWithItems.FirstOrDefault();
                if (orderEntity == null)
                {
                    return new GeneralResponse<OrderReadDTO>
                    {
                        Success = false,
                        Message = $"Order with ID '{id}' not found.",
                        Data = null,
                    };
                }

                return new GeneralResponse<OrderReadDTO>
                {
                    Success = true,
                    Message = "Order retrieved successfully.",
                    Data = OrderMapper.ToReadDTO(orderEntity),
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<OrderReadDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the order.",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<OrderReadDTO>>> GetAllOrdersAsync()
        {
            try
            {
                var allOrders = await _orderRepo.FindWithStringIncludesAsync(
                    o => true,
                    includeProperties: "OrderItems,OrderItems.Product,OrderItems.Product.Category,OrderItems.Product.SubCategory,OrderItems.Product.TechCompany,Customer,Customer.User,OrderHistory,ServiceUsage,Deliveries"
                );

                var orderDtos = allOrders
                    .Where(o => o != null)
                    .Select(OrderMapper.ToReadDTO)
                    .Where(dto => dto != null);

                return new GeneralResponse<IEnumerable<OrderReadDTO>>
                {
                    Success = true,
                    Message = "Orders retrieved successfully.",
                    Data = orderDtos,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<OrderReadDTO>>
                {
                    Success = false,
                    Message = $"An unexpected error occurred while retrieving orders.{ex}",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<OrderReadDTO>>> GetOrdersByCustomerIdAsync(
            string customerId
        )
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return new GeneralResponse<IEnumerable<OrderReadDTO>>
                {
                    Success = false,
                    Message = "Customer ID cannot be null or empty.",
                    Data = null,
                };
            }

            if (!Guid.TryParse(customerId, out _))
            {
                return new GeneralResponse<IEnumerable<OrderReadDTO>>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Expected GUID format.",
                    Data = null,
                };
            }

            try
            {
                var orders = await _orderRepo.FindWithStringIncludesAsync(
                    o => o.CustomerId == customerId,
                    includeProperties: "OrderItems,OrderItems.Product,OrderItems.Product.Category,OrderItems.Product.SubCategory,OrderItems.Product.TechCompany,Customer,Customer.User,OrderHistory,Delivery,ServiceUsage"
                );

                var orderDtos = orders
                    .Where(o => o != null)
                    .Select(OrderMapper.ToReadDTO)
                    .Where(dto => dto != null);

                return new GeneralResponse<IEnumerable<OrderReadDTO>>
                {
                    Success = true,
                    Message = "Customer orders retrieved successfully.",
                    Data = orderDtos,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<OrderReadDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving customer orders.",
                    Data = null,
                };
            }
        }

        private async Task<OrderHistory> GetOrCreateOrderHistoryAsync(string customerId)
        {
            var existingHistory = await _orderHistoryRepo.GetFirstOrDefaultAsync(
                oh => oh.Orders.Any(o => o.CustomerId == customerId),
                includeProperties: "Orders"
            );

            if (existingHistory != null)
            {
                return existingHistory;
            }

            var newHistory = new OrderHistory
            {
                Id = Guid.NewGuid().ToString(),
                Orders = new List<Order>(),
            };

            await _orderHistoryRepo.AddAsync(newHistory);
            await _orderHistoryRepo.SaveChangesAsync();

            return newHistory;
        }

        public async Task<
            GeneralResponse<IEnumerable<OrderHistoryReadDTO>>
        > GetOrderHistoryByCustomerIdAsync(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return new GeneralResponse<IEnumerable<OrderHistoryReadDTO>>
                {
                    Success = false,
                    Message = "Customer ID cannot be null or empty.",
                    Data = null,
                };
            }

            if (!Guid.TryParse(customerId, out _))
            {
                return new GeneralResponse<IEnumerable<OrderHistoryReadDTO>>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Expected GUID format.",
                    Data = null,
                };
            }

            try
            {
                var orderHistories = await _orderHistoryRepo.FindWithStringIncludesAsync(
                    oh => oh.Orders.Any(o => o.CustomerId == customerId),
                    includeProperties: "Orders,Orders.OrderItems,Orders.OrderItems.Product,Orders.Customer,Orders.Customer.User"
                );

                var orderHistoryDtos = orderHistories
                    .Where(oh => oh != null)
                    .Select(OrderHistoryMapper.MapToOrderHistoryReadDTO)
                    .Where(dto => dto != null);

                return new GeneralResponse<IEnumerable<OrderHistoryReadDTO>>
                {
                    Success = true,
                    Message = "Customer order history retrieved successfully.",
                    Data = orderHistoryDtos,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<OrderHistoryReadDTO>>
                {
                    Success = false,
                    Message =
                        "An unexpected error occurred while retrieving customer order history.",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<bool>> UpdateOrderStatusAsync(
            string orderId,
            OrderStatus newStatus
        )
        {
            if (string.IsNullOrWhiteSpace(orderId))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Order ID cannot be null or empty.",
                    Data = false,
                };
            }

            if (!Guid.TryParse(orderId, out _))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Invalid Order ID format. Expected GUID format.",
                    Data = false,
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
                        Data = false,
                    };
                }

                order.Status = newStatus;
                _orderRepo.Update(order);
                await _orderRepo.SaveChangesAsync();

                await _notificationService.SendNotificationAsync(
                    order.CustomerId,
                    NotificationType.OrderStatusChanged,
                    order.Id,
                    "Order",
                    order.Id,
                    newStatus.GetStringValue()
                );

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message =
                        $"Order status updated successfully to '{newStatus.GetStringValue()}'.",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"An unexpected error occurred while updating order status. {ex}",
                    Data = false,
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<OrderReadDTO>>> GetOrdersByStatusAsync(
            OrderStatus status
        )
        {
            try
            {
                var orders = await _orderRepo.FindWithStringIncludesAsync(
                    o => o.Status == status,
                    includeProperties: "OrderItems,OrderItems.Product,Customer,Customer.User,OrderHistory"
                );

                var orderDtos = orders
                    .Where(o => o != null)
                    .Select(OrderMapper.ToReadDTO)
                    .Where(dto => dto != null);

                return new GeneralResponse<IEnumerable<OrderReadDTO>>
                {
                    Success = true,
                    Message =
                        $"Orders with status '{status.GetStringValue()}' retrieved successfully.",
                    Data = orderDtos,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<OrderReadDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving orders by status.",
                    Data = null,
                };
            }
        }
    }
}
