using Core.DTOs.ServiceUsageDTOs;
using Core.Enums;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class ServiceUsageMapper
    {
        public static ServiceUsageReadDTO ToReadDTO(ServiceUsage entity) =>
            new()
            {
                Id = entity.Id,
                ServiceType = entity.ServiceType,
                UsedOn = entity.UsedOn,
                CallCount = entity.CallCount,
            };

        public static ServiceUsage ToEntity(ServiceUsageCreateDTO dto) =>
            new()
            {
                Id = Guid.NewGuid().ToString(),
                ServiceType = dto.ServiceType,
                UsedOn = dto.UsedOn,
                CallCount = dto.CallCount,
            };

        public static void UpdateEntity(ServiceUsage entity, ServiceUsageUpdateDTO dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.ServiceType.ToString()))
                entity.ServiceType = (ServiceType)dto.ServiceType;
            if (dto.UsedOn.HasValue)
                entity.UsedOn = dto.UsedOn.Value;
            if (dto.CallCount.HasValue)
                entity.CallCount = dto.CallCount.Value;
        }
    }
}
