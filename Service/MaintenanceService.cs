using Core.DTOs.MaintenanceDTOs;
using Core.DTOs;
using Core.Interfaces;
using Core.Interfaces.Services;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;
using Core.Enums;

namespace Service
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IRepository<Maintenance> _maintenanceRepo;
        private readonly IRepository<Customer> _customerRepo;
        private readonly IRepository<TechCompany> _techCompanyRepo;
        private readonly IRepository<Warranty> _warrantyRepo;
        private readonly IRepository<ServiceUsage> _serviceUsageRepo;
        private readonly INotificationService _notificationService;

        public MaintenanceService(
            IRepository<Maintenance> maintenanceRepo, IRepository<Customer> customerRepo, IRepository<TechCompany> techCompanyRepo, 
            IRepository<Warranty> warrantyRepo, IRepository<ServiceUsage> serviceUsageRepo, INotificationService notificationService)
        {
            _maintenanceRepo = maintenanceRepo;
            _customerRepo = customerRepo;
            _techCompanyRepo = techCompanyRepo;
            _warrantyRepo = warrantyRepo;
            _serviceUsageRepo = serviceUsageRepo;
            _notificationService = notificationService;
        }

        public async Task<GeneralResponse<IEnumerable<MaintenanceDTO>>> GetAllAsync()
        {
            try
            {
                // Optimized includes for maintenance listing with essential related data
                var maintenances = await _maintenanceRepo.GetAllWithIncludesAsync(
                    m => m.Customer,
                    m => m.Customer.User,
                    m => m.TechCompany,
                    m => m.TechCompany.User,
                    m => m.Warranty);

                var maintenanceDtos = maintenances.Select(MaintenanceMapper.MapToMaintenanceDTO).ToList();

                return new GeneralResponse<IEnumerable<MaintenanceDTO>>
                {
                    Success = true,
                    Message = "Maintenance requests retrieved successfully.",
                    Data = maintenanceDtos
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<MaintenanceDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving maintenance requests.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<MaintenanceDetailsDTO>> GetByIdAsync(string id)
        {
            
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<MaintenanceDetailsDTO>
                {
                    Success = false,
                    Message = "Maintenance ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<MaintenanceDetailsDTO>
                {
                    Success = false,
                    Message = "Invalid Maintenance ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                // Comprehensive includes for detailed maintenance view
                var maintenance = await _maintenanceRepo.GetByIdWithIncludesAsync(id,
                    m => m.Customer,
                    m => m.Customer.User,
                    m => m.TechCompany,
                    m => m.TechCompany.User,
                    m => m.Warranty,
                    m => m.ServiceUsages);

                if (maintenance == null)
                {
                    return new GeneralResponse<MaintenanceDetailsDTO>
                    {
                        Success = false,
                        Message = $"Maintenance request with ID '{id}' not found.",
                        Data = null
                    };
                }

                return new GeneralResponse<MaintenanceDetailsDTO>
                {
                    Success = true,
                    Message = "Maintenance request retrieved successfully.",
                    Data = MaintenanceMapper.MapToMaintenanceDetailsDTO(maintenance)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<MaintenanceDetailsDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the maintenance request.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<MaintenanceDTO>> AddAsync(MaintenanceCreateDTO dto)
        {
            
            if (dto == null)
            {
                return new GeneralResponse<MaintenanceDTO>
                {
                    Success = false,
                    Message = "Maintenance data cannot be null.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(dto.CustomerId))
            {
                return new GeneralResponse<MaintenanceDTO>
                {
                    Success = false,
                    Message = "Customer ID is required.",
                    Data = null
                };
            }

            if (!Guid.TryParse(dto.CustomerId, out _))
            {
                return new GeneralResponse<MaintenanceDTO>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Expected GUID format.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(dto.TechCompanyId))
            {
                return new GeneralResponse<MaintenanceDTO>
                {
                    Success = false,
                    Message = "Tech Company ID is required.",
                    Data = null
                };
            }

            if (!Guid.TryParse(dto.TechCompanyId, out _))
            {
                return new GeneralResponse<MaintenanceDTO>
                {
                    Success = false,
                    Message = "Invalid Tech Company ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                var entity = MaintenanceMapper.MapToMaintenance(dto);

                // Create a ServiceUsage record for this maintenance
                var serviceUsage = new ServiceUsage
                {
                    Id = Guid.NewGuid().ToString(),
                    ServiceType = "Maintenance",
                    UsedOn = DateTime.Now,
                    CallCount = 1,
                    MaintenanceId = entity.Id
                };

                entity.ServiceUsages = new List<ServiceUsage> { serviceUsage };

                await _maintenanceRepo.AddAsync(entity);
                await _serviceUsageRepo.AddAsync(serviceUsage);
                await _maintenanceRepo.SaveChangesAsync();

                // Send notification to TechCompany about new maintenance request
                await _notificationService.SendNotificationAsync(
                    entity.TechCompanyId,
                    NotificationType.MaintenanceRequestCreated,
                    entity.Id,
                    "Maintenance",
                    entity.Id,
                    entity.CustomerId
                );

                // Get the created maintenance with all includes to return proper names
                var createdMaintenance = await _maintenanceRepo.GetFirstOrDefaultAsync(
                    m => m.Id == entity.Id,
                    includeProperties: "Customer,Customer.User,TechCompany,TechCompany.User,Warranty,Warranty.Product,ServiceUsages"
                );

                var maintenanceDto = MaintenanceMapper.MapToMaintenanceDTO(createdMaintenance);

                return new GeneralResponse<MaintenanceDTO>
                {
                    Success = true,
                    Message = "Maintenance created successfully.",
                    Data = maintenanceDto
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<MaintenanceDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while creating the maintenance.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<bool>> UpdateAsync(string id, MaintenanceUpdateDTO dto)
        {
            
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Maintenance ID cannot be null or empty.",
                    Data = false
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Invalid Maintenance ID format. Expected GUID format.",
                    Data = false
                };
            }

            if (dto == null)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Update data cannot be null.",
                    Data = false
                };
            }

            if (string.IsNullOrWhiteSpace(dto.CustomerId))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Customer ID is required.",
                    Data = false
                };
            }

            if (!Guid.TryParse(dto.CustomerId, out _))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Invalid Customer ID format. Expected GUID format.",
                    Data = false
                };
            }

            if (string.IsNullOrWhiteSpace(dto.TechCompanyId))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Tech Company ID is required.",
                    Data = false
                };
            }

            if (!Guid.TryParse(dto.TechCompanyId, out _))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Invalid Tech Company ID format. Expected GUID format.",
                    Data = false
                };
            }

            try
            {
                var maintenance = await _maintenanceRepo.GetByIdAsync(id);
                if (maintenance == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = $"Maintenance with ID '{id}' not found.",
                        Data = false
                    };
                }

                var customerTask = _customerRepo.GetByIdAsync(dto.CustomerId);
                var techCompanyTask = _techCompanyRepo.GetByIdAsync(dto.TechCompanyId);
                var warrantyTask = _warrantyRepo.GetByIdAsync(dto.WarrantyId);

                await Task.WhenAll(customerTask, techCompanyTask, warrantyTask);

                if (customerTask.Result == null || techCompanyTask.Result == null ||
                    warrantyTask.Result == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "One or more related entities (Customer, TechCompany, Warranty, ServiceUsage) not found.",
                        Data = false
                    };
                }

                MaintenanceMapper.MapToMaintenance(dto, maintenance);

                _maintenanceRepo.Update(maintenance);
                await _maintenanceRepo.SaveChangesAsync();

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Maintenance updated successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating the maintenance.",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id)
        {
            
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Maintenance ID cannot be null or empty.",
                    Data = false
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Invalid Maintenance ID format. Expected GUID format.",
                    Data = false
                };
            }

            try
            {
                var entity = await _maintenanceRepo.GetByIdAsync(id);
                if (entity == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = $"Maintenance with ID '{id}' not found.",
                        Data = false
                    };
                }

                _maintenanceRepo.Remove(entity);
                await _maintenanceRepo.SaveChangesAsync();
                
                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Maintenance deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "An unexpected error occurred while deleting the maintenance.",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<MaintenanceDTO>>> GetByTechCompanyIdAsync(string techCompanyId)
        {
            if (string.IsNullOrWhiteSpace(techCompanyId))
            {
                return new GeneralResponse<IEnumerable<MaintenanceDTO>>
                {
                    Success = false,
                    Message = "Tech Company ID cannot be null or empty.",
                    Data = null
                };
            }

            try
            {
                var maintenances = await _maintenanceRepo.FindWithStringIncludesAsync(
                    m => m.TechCompanyId == techCompanyId,
                    includeProperties: "Customer,Customer.User,TechCompany,TechCompany.User,Warranty,Warranty.Product,ServiceUsages"
                );

                return new GeneralResponse<IEnumerable<MaintenanceDTO>>
                {
                    Success = true,
                    Message = "Maintenances for tech company retrieved successfully.",
                    Data = maintenances.Select(MaintenanceMapper.MapToMaintenanceDTO)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<MaintenanceDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving maintenances for tech company.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<MaintenanceDTO>> AcceptMaintenanceRequestAsync(string maintenanceId, string techCompanyId)
        {
            if (string.IsNullOrWhiteSpace(maintenanceId) || string.IsNullOrWhiteSpace(techCompanyId))
            {
                return new GeneralResponse<MaintenanceDTO>
                {
                    Success = false,
                    Message = "Maintenance ID and Tech Company ID cannot be null or empty.",
                    Data = null
                };
            }

            try
            {
                var maintenance = await _maintenanceRepo.GetFirstOrDefaultAsync(
                    m => m.Id == maintenanceId,
                    includeProperties: "Customer,Customer.User,TechCompany,TechCompany.User,Warranty,Warranty.Product,ServiceUsages"
                );
                if (maintenance == null)
                {
                    return new GeneralResponse<MaintenanceDTO>
                    {
                        Success = false,
                        Message = "Maintenance request not found.",
                        Data = null
                    };
                }

                if (maintenance.TechCompanyId != null)
                {
                    return new GeneralResponse<MaintenanceDTO>
                    {
                        Success = false,
                        Message = "This maintenance request has already been assigned to a tech company.",
                        Data = null
                    };
                }

                // Ensure the maintenance has a ServiceUsage record
                await EnsureMaintenanceHasServiceUsage(maintenance);

                maintenance.TechCompanyId = techCompanyId;
                maintenance.Status = MaintenanceStatus.InProgress;
                _maintenanceRepo.Update(maintenance);
                await _maintenanceRepo.SaveChangesAsync();

                await _notificationService.SendNotificationAsync(
                   maintenance.CustomerId,
                   NotificationType.MaintenanceRequestAccepted,
                   maintenance.Id,
                   "Maintenance",
                   maintenance.Id,
                   maintenance.TechCompanyId
               );

                return new GeneralResponse<MaintenanceDTO>
                {
                    Success = true,
                    Message = "Maintenance request accepted successfully.",
                    Data = MaintenanceMapper.MapToMaintenanceDTO(maintenance)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<MaintenanceDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while accepting maintenance request.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<MaintenanceDTO>> CompleteMaintenanceAsync(string maintenanceId, string techCompanyId, string notes)
        {
            if (string.IsNullOrWhiteSpace(maintenanceId) || string.IsNullOrWhiteSpace(techCompanyId))
            {
                return new GeneralResponse<MaintenanceDTO>
                {
                    Success = false,
                    Message = "Maintenance ID and Tech Company ID cannot be null or empty.",
                    Data = null
                };
            }

            try
            {
                var maintenance = await _maintenanceRepo.GetFirstOrDefaultAsync(
                    m => m.Id == maintenanceId,
                    includeProperties: "Customer,Customer.User,TechCompany,TechCompany.User,Warranty,Warranty.Product,ServiceUsages"
                );
                if (maintenance == null)
                {
                    return new GeneralResponse<MaintenanceDTO>
                    {
                        Success = false,
                        Message = "Maintenance request not found.",
                        Data = null
                    };
                }

                if (maintenance.TechCompanyId != techCompanyId)
                {
                    return new GeneralResponse<MaintenanceDTO>
                    {
                        Success = false,
                        Message = "This maintenance request is not assigned to the specified tech company.",
                        Data = null
                    };
                }

                // Ensure the maintenance has a ServiceUsage record
                await EnsureMaintenanceHasServiceUsage(maintenance);

                maintenance.Status = MaintenanceStatus.Completed;
                maintenance.Notes = notes;
                maintenance.CompletedDate = DateTime.Now;
                _maintenanceRepo.Update(maintenance);
                await _maintenanceRepo.SaveChangesAsync();

                // Send notification to customer about maintenance completion
                await _notificationService.SendNotificationAsync(
                maintenance.CustomerId,
                NotificationType.MaintenanceRequestCompleted,
                maintenance.Id,
                "Maintenance",
                maintenance.Id,
                maintenance.TechCompanyId
                );

                return new GeneralResponse<MaintenanceDTO>
                {
                    Success = true,
                    Message = "Maintenance completed successfully.",
                    Data = MaintenanceMapper.MapToMaintenanceDTO(maintenance)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<MaintenanceDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while completing maintenance.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<MaintenanceDTO>>> GetAvailableMaintenanceRequestsAsync()
        {
            try
            {
                var maintenances = await _maintenanceRepo.FindWithStringIncludesAsync(
                    m => m.TechCompanyId == null && m.Status == MaintenanceStatus.Requested,
                    includeProperties: "Customer,Customer.User,Warranty,Warranty.Product,ServiceUsages"
                );

                return new GeneralResponse<IEnumerable<MaintenanceDTO>>
                {
                    Success = true,
                    Message = "Available maintenance requests retrieved successfully.",
                    Data = maintenances.Select(MaintenanceMapper.MapToMaintenanceDTO)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<MaintenanceDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving available maintenance requests.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<MaintenanceDTO>> UpdateMaintenanceStatusAsync(string maintenanceId, string newStatus)
        {
            if (string.IsNullOrWhiteSpace(maintenanceId) || string.IsNullOrWhiteSpace(newStatus))
            {
                return new GeneralResponse<MaintenanceDTO>
                {
                    Success = false,
                    Message = "Maintenance ID and new status cannot be null or empty.",
                    Data = null
                };
            }

            try
            {
                var maintenance = await _maintenanceRepo.GetFirstOrDefaultAsync(
                    m => m.Id == maintenanceId,
                    includeProperties: "Customer,Customer.User,TechCompany,TechCompany.User,Warranty,Warranty.Product,ServiceUsages"
                );
                if (maintenance == null)
                {
                    return new GeneralResponse<MaintenanceDTO>
                    {
                        Success = false,
                        Message = "Maintenance request not found.",
                        Data = null
                    };
                }

                // Ensure the maintenance has a ServiceUsage record
                await EnsureMaintenanceHasServiceUsage(maintenance);

                // Parse the string status to enum
                if (Enum.TryParse<MaintenanceStatus>(newStatus, out var status))
                {
                    maintenance.Status = status;
                }
                else
                {
                    return new GeneralResponse<MaintenanceDTO>
                    {
                        Success = false,
                        Message = $"Invalid status: {newStatus}",
                        Data = null
                    };
                }
                _maintenanceRepo.Update(maintenance);
                await _maintenanceRepo.SaveChangesAsync();

                return new GeneralResponse<MaintenanceDTO>
                {
                    Success = true,
                    Message = $"Maintenance status updated to '{newStatus}' successfully.",
                    Data = MaintenanceMapper.MapToMaintenanceDTO(maintenance)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<MaintenanceDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating maintenance status.",
                    Data = null
                };
            }
        }

        private async Task EnsureMaintenanceHasServiceUsage(Maintenance maintenance)
        {
            // If maintenance doesn't have any ServiceUsage records, create one
            if (maintenance.ServiceUsages == null || !maintenance.ServiceUsages.Any())
            {
                var serviceUsage = new ServiceUsage
                {
                    Id = Guid.NewGuid().ToString(),
                    ServiceType = "Maintenance",
                    UsedOn = DateTime.Now,
                    CallCount = 1,
                    MaintenanceId = maintenance.Id
                };

                await _serviceUsageRepo.AddAsync(serviceUsage);
                maintenance.ServiceUsages = new List<ServiceUsage> { serviceUsage };
            }
        }

        public async Task<GeneralResponse<MaintenanceNearestDTO>> GetNearestMaintenanceAsync(string customerId)
        {
            try
            {
                var customer = await _customerRepo.GetFirstOrDefaultAsync(
                    c => c.Id == customerId,
                    includeProperties: "User"
                );

                if (customer == null)
                {
                    return new GeneralResponse<MaintenanceNearestDTO>
                    {
                        Success = false,
                        Message = "Customer not found."
                    };
                }

                // Get all tech companies that provide maintenance services
                var techCompanies = await _techCompanyRepo.GetAllAsync();
                var maintenanceTechCompanies = techCompanies.ToList();

                if (!maintenanceTechCompanies.Any())
                {
                    return new GeneralResponse<MaintenanceNearestDTO>
                    {
                        Success = false,
                        Message = "No maintenance centers found."
                    };
                }

                // Simple distance calculation based on address matching
                // In a real implementation, you would use geocoding and distance calculation
                var customerAddress = customer.User?.Address?.ToLower() ?? "";
                var customerRegion = ExtractRegionFromAddress(customerAddress);
                var customerPostalCode = ExtractPostalCodeFromAddress(customerAddress);

                var nearestMaintenance = maintenanceTechCompanies
                    .Select(tc => new
                    {
                        TechCompany = tc,
                        Distance = CalculateDistance(customerAddress, customerRegion, customerPostalCode, tc.User?.Address ?? "")
                    })
                    .OrderBy(x => x.Distance)
                    .First();

                var result = new MaintenanceNearestDTO
                {
                    Id = nearestMaintenance.TechCompany.Id,
                    Name = nearestMaintenance.TechCompany.User?.FullName ?? "Unknown",
                    Address = nearestMaintenance.TechCompany.User?.Address ?? "Unknown",
                    TechCompanyName = nearestMaintenance.TechCompany.User?.FullName ?? "Unknown",
                    TechCompanyAddress = nearestMaintenance.TechCompany.User?.Address ?? "Unknown",
                    TechCompanyPhone = nearestMaintenance.TechCompany.User?.PhoneNumber ?? "Unknown",
                    Distance = nearestMaintenance.Distance,
                    Region = ExtractRegionFromAddress(nearestMaintenance.TechCompany.User?.Address ?? ""),
                    PostalCode = ExtractPostalCodeFromAddress(nearestMaintenance.TechCompany.User?.Address ?? "")
                };

                return new GeneralResponse<MaintenanceNearestDTO>
                {
                    Success = true,
                    Message = "Nearest maintenance center found successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<MaintenanceNearestDTO>
                {
                    Success = false,
                    Message = "An error occurred while finding nearest maintenance center.",
                    Data = null
                };
            }
        }

        private string ExtractRegionFromAddress(string address)
        {
            // Simple region extraction - can be enhanced with more sophisticated logic
            var parts = address.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 1 ? parts[1].Trim() : "";
        }

        private string ExtractPostalCodeFromAddress(string address)
        {
            // Simple postal code extraction - can be enhanced with regex patterns
            var parts = address.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 2 ? parts[2].Trim() : "";
        }

        private double CalculateDistance(string customerAddress, string customerRegion, string customerPostalCode, string techCompanyAddress)
        {
            // Simple distance calculation based on address matching
            // In a real implementation, you would use geocoding APIs and calculate actual distances
            
            var techCompanyRegion = ExtractRegionFromAddress(techCompanyAddress);
            var techCompanyPostalCode = ExtractPostalCodeFromAddress(techCompanyAddress);

            // If same region, give lower distance
            if (customerRegion.Equals(techCompanyRegion, StringComparison.OrdinalIgnoreCase))
            {
                return 5.0; // 5 km
            }

            // If same postal code, give very low distance
            if (customerPostalCode.Equals(techCompanyPostalCode, StringComparison.OrdinalIgnoreCase))
            {
                return 1.0; // 1 km
            }

            // Default distance
            return 50.0; // 50 km
        }
    }
}
