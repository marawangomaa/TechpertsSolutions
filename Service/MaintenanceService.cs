using Core.DTOs.MaintenanceDTOs;
using TechpertsSolutions.Core.DTOs;
using Core.Entities;
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
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IRepository<Maintenance> _maintenanceRepo;
        private readonly IRepository<Customer> _customerRepo;
        private readonly IRepository<TechCompany> _techCompanyRepo;
        private readonly IRepository<Warranty> _warrantyRepo;
        private readonly IRepository<ServiceUsage> _serviceUsageRepo;

        public MaintenanceService(
            IRepository<Maintenance> maintenanceRepo, IRepository<Customer> customerRepo, IRepository<TechCompany> techCompanyRepo, 
            IRepository<Warranty> warrantyRepo, IRepository<ServiceUsage> serviceUsageRepo)
        {
            _maintenanceRepo = maintenanceRepo;
            _customerRepo = customerRepo;
            _techCompanyRepo = techCompanyRepo;
            _warrantyRepo = warrantyRepo;
            _serviceUsageRepo = serviceUsageRepo;
        }

        public async Task<GeneralResponse<IEnumerable<MaintenanceDTO>>> GetAllAsync()
        {
            try
            {
                var maintenances = await _maintenanceRepo.FindWithStringIncludesAsync(
                    m => true,
                    includeProperties: "Customer,Customer.User,TechCompany,TechCompany.User,Warranty,Warranty.Product,ServiceUsages"
                );
                return new GeneralResponse<IEnumerable<MaintenanceDTO>>
                {
                    Success = true,
                    Message = "Maintenances retrieved successfully.",
                    Data = MaintenanceMapper.MapToMaintenanceDTOList(maintenances)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<MaintenanceDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving maintenances.",
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
                var maintenance = await _maintenanceRepo.GetFirstOrDefaultAsync(
                    m => m.Id == id,
                    includeProperties: "Customer,Customer.User,TechCompany,TechCompany.User,Warranty,Warranty.Product,ServiceUsages"
                );
                if (maintenance == null)
                {
                    return new GeneralResponse<MaintenanceDetailsDTO>
                    {
                        Success = false,
                        Message = $"Maintenance with ID '{id}' not found.",
                        Data = null
                    };
                }

                // Ensure the maintenance has a ServiceUsage record
                await EnsureMaintenanceHasServiceUsage(maintenance);

                return new GeneralResponse<MaintenanceDetailsDTO>
                {
                    Success = true,
                    Message = "Maintenance retrieved successfully.",
                    Data = MaintenanceMapper.MapToMaintenanceDetailsDTO(maintenance)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<MaintenanceDetailsDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the maintenance.",
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
                    UsedOn = DateTime.UtcNow,
                    CallCount = 1,
                    MaintenanceId = entity.Id
                };

                entity.ServiceUsages = new List<ServiceUsage> { serviceUsage };

                await _maintenanceRepo.AddAsync(entity);
                await _serviceUsageRepo.AddAsync(serviceUsage);
                await _maintenanceRepo.SaveChangesAsync();

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
                maintenance.Status = "InProgress";
                _maintenanceRepo.Update(maintenance);
                await _maintenanceRepo.SaveChangesAsync();

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

                maintenance.Status = "Completed";
                maintenance.Notes = notes;
                maintenance.CompletedDate = DateTime.UtcNow;
                _maintenanceRepo.Update(maintenance);
                await _maintenanceRepo.SaveChangesAsync();

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
                    m => m.TechCompanyId == null && m.Status == "Pending",
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

                maintenance.Status = newStatus;
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
                    UsedOn = DateTime.UtcNow,
                    CallCount = 1,
                    MaintenanceId = maintenance.Id
                };

                await _serviceUsageRepo.AddAsync(serviceUsage);
                maintenance.ServiceUsages = new List<ServiceUsage> { serviceUsage };
            }
        }
    }
}
