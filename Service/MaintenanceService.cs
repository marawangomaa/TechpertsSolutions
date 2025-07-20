using Core.DTOs.Maintenance;
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
                var maintenances = await _maintenanceRepo.GetAllAsync();
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
            // Input validation
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
                var maintenance = await _maintenanceRepo.GetByIdAsync(id);
                if (maintenance == null)
                {
                    return new GeneralResponse<MaintenanceDetailsDTO>
                    {
                        Success = false,
                        Message = $"Maintenance with ID '{id}' not found.",
                        Data = null
                    };
                }

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
            // Input validation
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

                await _maintenanceRepo.AddAsync(entity);
                await _maintenanceRepo.SaveChangesAsync();

                var customer = await _customerRepo.GetByIdAsync(dto.CustomerId);
                var techCompany = await _techCompanyRepo.GetByIdAsync(dto.TechCompanyId);
                var warranty = await _warrantyRepo.GetByIdAsync(dto.WarrantyId);
                var serviceUsage = await _serviceUsageRepo.GetByIdAsync(dto.ServiceUsageId);

                var maintenanceDto = MaintenanceMapper.MapToMaintenanceDTO(entity, 
                    customer?.User?.FullName ?? "Unknown",
                    techCompany?.User?.FullName ?? "Unknown",
                    warranty?.Product?.Name ?? "Unknown",
                    serviceUsage?.ServiceType ?? "Unknown",
                    warranty?.StartDate ?? DateTime.MinValue,
                    warranty?.EndDate ?? DateTime.MinValue);

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
            // Input validation
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
                var serviceUsageTask = _serviceUsageRepo.GetByIdAsync(dto.ServiceUsageId);

                await Task.WhenAll(customerTask, techCompanyTask, warrantyTask, serviceUsageTask);

                if (customerTask.Result == null || techCompanyTask.Result == null ||
                    warrantyTask.Result == null || serviceUsageTask.Result == null)
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
            // Input validation
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
    }
}
