using Core.DTOs.DeliveryDTOs;
using Core.Entities;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class DeliveryMapper
    {
        public static DeliveryDTO MapToDeliveryDTO(Delivery delivery)
        {
            if (delivery == null) return null;

            return new DeliveryDTO
            {
                Id = delivery.Id
            };
        }

        public static Delivery MapToDelivery()
        {
            return new Delivery
            {
                Id = Guid.NewGuid().ToString()
            };
        }

        public static DeliveryDetailsDTO MapToDeliveryDetailsDTO(Delivery delivery)
        {
            if (delivery == null) return null;

            return new DeliveryDetailsDTO
            {
                Id = delivery.Id,
                Customers = delivery.Customers?.Select(c => new DeliveryCustomerDTO
                {
                    Id = c.Id,
                    City = c.City,
                    Country = c.Country,
                    UserFullName = c.User?.FullName
                }).ToList(),

                Orders = delivery.Orders?.Select(o => new DeliveryOrderDTO
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    CustomerName = o.Customer?.User?.FullName ?? "",
                    City = o.Customer?.City,
                    Status = o.Status
                }).ToList(),

                TechCompanies = delivery.TechCompanies?.Select(t => new DeliveryTechCompanyDTO
                {
                    Id = t.Id,
                    City = t.City,
                    Country = t.Country,
                    UserFullName = t.User?.FullName
                }).ToList()
            };
        }

        public static IEnumerable<DeliveryDTO> MapToDeliveryDTOList(IEnumerable<Delivery> deliveries)
        {
            if (deliveries == null) return Enumerable.Empty<DeliveryDTO>();

            return deliveries.Select(MapToDeliveryDTO).Where(dto => dto != null);
        }
    }
} 