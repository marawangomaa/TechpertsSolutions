using Core.DTOs.MaintenanceDTOs;
using Core.DTOs.MaintenanceDTOss;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class MaintenanceMapper
    {
        public static MaintenanceDTO MapToMaintenanceDTO(Maintenance maintenance)
        {
            if (maintenance == null)
                return null;

            ServiceType serviceType = ServiceType.Maintenance;

            if (maintenance.ServiceUsages != null && maintenance.ServiceUsages.Any())
            {
                var firstServiceUsage = maintenance.ServiceUsages.FirstOrDefault();
                if (
                    firstServiceUsage != null
                    && !string.IsNullOrEmpty(firstServiceUsage.ServiceType.ToString())
                )
                {
                    serviceType = firstServiceUsage.ServiceType;
                }
            }

            return new MaintenanceDTO
            {
                Id = maintenance.Id,
                CustomerName = maintenance.Customer?.User?.FullName ?? "Unknown Customer",
                TechCompanyName = maintenance.TechCompany?.User?.FullName ?? "Unknown Tech Company",
                ProductName = maintenance.Warranty?.Product?.Name ?? "Unknown Product",
                DeviceType = maintenance.DeviceType,
                ServiceType = serviceType,
                Notes = maintenance.Notes,
                Issue = maintenance.Issue,
                DeviceImages = maintenance.DeviceImages,
                WarrantyStart = maintenance.Warranty?.StartDate ?? DateTime.MinValue,
                WarrantyEnd = maintenance.Warranty?.EndDate ?? DateTime.MinValue,
                CompletedDate = maintenance.CompletedDate,
                Priority = maintenance.Priority
            };
        }

        public static async Task<Maintenance> MapToMaintenance(MaintenanceCreateDTO dto, ITechCompanyService techCompanyService, UserManager<AppUser> _userService)
        {
            if (dto == null)
                return null;

            // Step 1: Get the user by the RoleId (or whatever identifier you currently have)
            var user = await _userService.FindByIdAsync(dto.TechCompanyId); // or appropriate method
            if (user == null)
                throw new Exception($"User not found for RoleId: {dto.TechCompanyId}");

            // Step 2: Get the tech company from the user
            var techCompany = await techCompanyService.GetByUserId(user.Id);
            if (techCompany == null || techCompany.Data == null)
                throw new Exception($"Tech company not found for User ID: {user.Id}");

            return new Maintenance
            {
                CustomerId = dto.CustomerId,
                TechCompanyId = techCompany.Data.Id,
                WarrantyId = dto.WarrantyId,
                Notes = dto.Notes,
                Priority = dto.Priority,
                DeviceImages = dto.DeviceImages,
                DeviceType = dto.DeviceType,
                Issue = dto.Issue
            };
        }


        public static Maintenance MapToMaintenance(
            MaintenanceUpdateDTO dto,
            Maintenance existingMaintenance
        )
        {
            if (dto == null || existingMaintenance == null)
                return null;

            existingMaintenance.CustomerId = dto.CustomerId;
            existingMaintenance.TechCompanyId = dto.TechCompanyId;
            existingMaintenance.WarrantyId = dto.WarrantyId;
            existingMaintenance.Notes = dto.Notes;
            existingMaintenance.Status = dto.Status.Value;
            existingMaintenance.Issue = dto.Issue;
            existingMaintenance.ServiceFee = dto.ServiceFee;
            existingMaintenance.CompletedDate = dto.CompletedDate;
            existingMaintenance.DeviceType = dto.DeviceType;
            existingMaintenance.DeviceImages = dto.DeviceImages;
            existingMaintenance.Priority = dto.Priority;
            return existingMaintenance;
        }

        public static MaintenanceDetailsDTO MapToMaintenanceDetailsDTO(Maintenance maintenance)
        {
            if (maintenance == null)
                return null;

            return new MaintenanceDetailsDTO
            {
                Id = maintenance.Id,

                Customer =
                    maintenance.Customer == null
                        ? null
                        : new MaintenanceCustomerDTO
                        {
                            Id = maintenance.Customer.Id,
                            FullName = maintenance.Customer.User?.FullName,
                        },

                TechCompany =
                    maintenance.TechCompany == null
                        ? null
                        : new MaintenanceTechCompanyDTO
                        {
                            Id = maintenance.TechCompany.Id,
                            FullName = maintenance.TechCompany.User?.FullName,
                        },

                Warranty =
                    maintenance.Warranty == null
                        ? null
                        : new MaintenanceWarrantyDTO
                        {
                            Id = maintenance.Warranty.Id,
                            Description = maintenance.Warranty.Description,
                            StartDate = maintenance.Warranty.StartDate,
                            EndDate = maintenance.Warranty.EndDate,
                            ProductName = maintenance.Warranty.Product?.Name,
                        },

                Product =
                    maintenance.Warranty?.Product == null
                        ? null
                        : new MaintenanceProductDTO
                        {
                            Id = maintenance.Warranty.Product.Id,
                            Name = maintenance.Warranty.Product.Name,
                            Price = maintenance.Warranty.Product.Price,
                        },

                ServiceUsage =
                    maintenance.ServiceUsages?.FirstOrDefault() == null
                        ? null
                        : new MaintenanceServiceUsageDTO
                        {
                            Id = maintenance.ServiceUsages.FirstOrDefault().Id,
                            ServiceType = maintenance.ServiceUsages.FirstOrDefault().ServiceType.ToString(),
                            UsedOn = maintenance.ServiceUsages.FirstOrDefault().UsedOn,
                            CallCount = maintenance.ServiceUsages.FirstOrDefault().CallCount,
                        },
                ServiceFee = maintenance.ServiceFee,
                Issue = maintenance.Issue,
                DeviceImages = maintenance.DeviceImages,
                DeviceType = maintenance.DeviceType,
                CompletedDate = maintenance.CompletedDate,
                Priority = maintenance.Priority,
                Notes = maintenance.Notes,
            };
        }
    }
}
