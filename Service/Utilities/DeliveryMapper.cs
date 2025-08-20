using Core.DTOs.DeliveryDTOs;
using Core.DTOs.DeliveryPersonDTOs;
using Core.DTOs.OrderDTOs;
using Core.Entities;
using Core.Enums;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class DeliveryMapper
    {
        public static Delivery ToEntity(DeliveryCreateDTO dto)
        {
            if (dto == null)
                return null;

            return new Delivery
            {

                OrderId = dto.OrderId,
                CustomerId = dto.customerId,
                Status = DeliveryStatus.Pending,
                CreatedAt = DateTime.Now,
                DropoffLatitude = dto.CustomerLatitude,
                DropoffLongitude = dto.CustomerLongitude,
            };
        }

        public static DeliveryReadDTO ToReadDTO(
            Delivery entity,
            IEnumerable<DeliveryClusterDTO>? clusters = null
        )
        {
            if (entity == null)
                return null;

            return new DeliveryReadDTO
            {
                Id = entity.Id,
                Status = entity.Status,
                DeliveryFee = entity.DeliveryFee,
                CreatedAt = entity.CreatedAt,
                Clusters = clusters?.ToList() ?? new List<DeliveryClusterDTO>(),
            };
        }

        public static OrderStatus ToOrderStatus(DeliveryStatus deliveryStatus)
        {
            return deliveryStatus switch
            {
                DeliveryStatus.Pending => OrderStatus.Pending,
                DeliveryStatus.Assigned => OrderStatus.Dispatched,
                DeliveryStatus.PickedUp => OrderStatus.InProgress,
                DeliveryStatus.Delivered => OrderStatus.Delivered,
                DeliveryStatus.Cancelled => OrderStatus.Cancelled,
                DeliveryStatus.Failed => OrderStatus.Rejected,
                _ => OrderStatus.Pending
            };
        }


        public static DeliveryDetailsDTO ToDetailsDTO(
            Delivery entity,
            IEnumerable<DeliveryClusterDTO>? clusters = null
        )
        {
            if (entity == null)
                return null;

            return new DeliveryDetailsDTO
            {
                Id = entity.Id,
                TrackingNumber = entity.TrackingNumber,
                DeliveryAddress = null,
                CustomerPhone = entity.CustomerPhone,
                CustomerName = entity.CustomerName,
                EstimatedDeliveryDate = entity.EstimatedDeliveryDate,
                ActualDeliveryDate = entity.ActualDeliveryDate,
                DeliveryStatus = entity.Status,
                Notes = entity.Notes,
                DeliveryFee = entity.DeliveryFee,
                DeliveryPerson =
                    entity.DeliveryPerson == null
                        ? null
                        : new DeliveryPersonDTO
                        {
                            Id = entity.DeliveryPerson.Id,
                            UserFullName = entity?.DeliveryPerson?.User?.FullName,
                            VehicleNumber = entity.DeliveryPerson.VehicleNumber,
                            VehicleType = entity.DeliveryPerson.VehicleType,
                            PhoneNumber = entity.DeliveryPerson.User?.PhoneNumber,
                            City = entity.DeliveryPerson.User?.City,
                            Country = entity.DeliveryPerson.User?.Country,
                            IsAvailable = entity.DeliveryPerson.IsAvailable,
                        },
                Order = null,
                TechCompanies = null,
                Clusters = clusters?.ToList() ?? new List<DeliveryClusterDTO>(),
            };
        }

        public static void UpdateEntity(Delivery entity, DeliveryUpdateDTO dto)
        {
            if (entity == null || dto == null)
                return;

            entity.Status = dto.Status;

            if (dto.DeliveryFee.HasValue)
                entity.DeliveryFee = dto.DeliveryFee.Value;

            entity.UpdatedAt = DateTime.Now;
        }
    }
}