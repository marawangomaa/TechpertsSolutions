using Core.DTOs.Orders;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class OrderHistoryMapper
    {
        public static OrderHistoryReadDTO MapToOrderHistoryReadDTO(OrderHistory orderHistory)
        {
            if (orderHistory == null) return null;

            return new OrderHistoryReadDTO
            {
                Id = orderHistory.Id ?? string.Empty,
                Orders = orderHistory.Orders?.Where(o => o != null)
                                         .Select(OrderMapper.ToReadDTO)
                                         .Where(dto => dto != null)
                                         .ToList() ?? new List<OrderReadDTO>()
            };
        }

        public static OrderHistoryReadDTO MapToOrderHistoryReadDTO(OrderHistory orderHistory, string customerId)
        {
            if (orderHistory == null) return null;

            return new OrderHistoryReadDTO
            {
                Id = orderHistory.Id ?? string.Empty,
                Orders = orderHistory.Orders?.Where(o => o != null && o.CustomerId == customerId)
                                         .Select(OrderMapper.ToReadDTO)
                                         .Where(dto => dto != null)
                                         .ToList() ?? new List<OrderReadDTO>()
            };
        }

        public static IEnumerable<OrderHistoryReadDTO> MapToOrderHistoryReadDTOList(IEnumerable<OrderHistory> orderHistories)
        {
            if (orderHistories == null) return Enumerable.Empty<OrderHistoryReadDTO>();

            return orderHistories.Select(MapToOrderHistoryReadDTO).Where(dto => dto != null);
        }

        public static OrderHistoryReadDTO CreateEmptyOrderHistoryReadDTO()
        {
            return new OrderHistoryReadDTO
            {
                Id = string.Empty,
                Orders = new List<OrderReadDTO>()
            };
        }
    }
} 