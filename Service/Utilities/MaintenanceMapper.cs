using Core.DTOs.MaintenanceDTOs;
using Core.Entities;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class MaintenanceMapper
    {
        public static MaintenanceDTO MapToMaintenanceDTO(Maintenance maintenance)
        {
            if (maintenance == null) return null;

            return new MaintenanceDTO
            {
                Id = maintenance.Id
            };
        }

        public static MaintenanceDTO MapToMaintenanceDTO(Maintenance maintenance, string customerName, string techCompanyName, string productName, string serviceType, DateTime warrantyStart, DateTime warrantyEnd)
        {
            if (maintenance == null) return null;

            return new MaintenanceDTO
            {
                Id = maintenance.Id,
                CustomerName = customerName,
                TechCompanyName = techCompanyName,
                ProductName = productName,
                ServiceType = serviceType,
                WarrantyStart = warrantyStart,
                WarrantyEnd = warrantyEnd
            };
        }

        public static Maintenance MapToMaintenance(MaintenanceCreateDTO dto)
        {
            if (dto == null) return null;

            return new Maintenance
            {
                Id = Guid.NewGuid().ToString(),
                CustomerId = dto.CustomerId,
                TechCompanyId = dto.TechCompanyId,
                WarrantyId = dto.WarrantyId
            };
        }

        public static Maintenance MapToMaintenance(MaintenanceUpdateDTO dto, Maintenance existingMaintenance)
        {
            if (dto == null || existingMaintenance == null) return null;

            existingMaintenance.CustomerId = dto.CustomerId;
            existingMaintenance.TechCompanyId = dto.TechCompanyId;
            existingMaintenance.WarrantyId = dto.WarrantyId;
            return existingMaintenance;
        }

        public static MaintenanceDetailsDTO MapToMaintenanceDetailsDTO(Maintenance maintenance)
        {
            if (maintenance == null) return null;

            return new MaintenanceDetailsDTO
            {
                Id = maintenance.Id,

                Customer = maintenance.Customer == null ? null : new MaintenanceCustomerDTO
                {
                    Id = maintenance.Customer.Id,
                    FullName = maintenance.Customer.User?.FullName
                },

                TechCompany = maintenance.TechCompany == null ? null : new MaintenanceTechCompanyDTO
                {
                    Id = maintenance.TechCompany.Id,
                    FullName = maintenance.TechCompany.User?.FullName
                },

                Warranty = maintenance.Warranty == null ? null : new MaintenanceWarrantyDTO
                {
                    Id = maintenance.Warranty.Id,
                    Description = maintenance.Warranty.Description,
                    StartDate = maintenance.Warranty.StartDate,
                    EndDate = maintenance.Warranty.EndDate,
                    ProductName = maintenance.Warranty.Product?.Name
                },

                Product = maintenance.Warranty?.Product == null ? null : new MaintenanceProductDTO
                {
                    Id = maintenance.Warranty.Product.Id,
                    Name = maintenance.Warranty.Product.Name,
                    Price = maintenance.Warranty.Product.Price
                },

                ServiceUsage = maintenance.ServiceUsages?.FirstOrDefault() == null ? null : new MaintenanceServiceUsageDTO
                {
                    Id = maintenance.ServiceUsages.FirstOrDefault().Id,
                    ServiceType = maintenance.ServiceUsages.FirstOrDefault().ServiceType,
                    UsedOn = maintenance.ServiceUsages.FirstOrDefault().UsedOn,
                    CallCount = maintenance.ServiceUsages.FirstOrDefault().CallCount
                }
            };
        }

        public static IEnumerable<MaintenanceDTO> MapToMaintenanceDTOList(IEnumerable<Maintenance> maintenances)
        {
            if (maintenances == null) return Enumerable.Empty<MaintenanceDTO>();

            return maintenances.Select(MapToMaintenanceDTO).Where(dto => dto != null);
        }
    }
} 
