using Core.DTOs;
using Core.DTOs.ServiceUsageDTOs;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Utilities;
using Service.Utilities;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class ServiceUsageService : IServiceUsageService
    {
        private readonly IRepository<ServiceUsage> _ServiceRepo;

        public ServiceUsageService(IRepository<ServiceUsage> repo) => _ServiceRepo = repo;

        public async Task<GeneralResponse<ServiceUsageReadDTO>> CreateAsync(
            ServiceUsageCreateDTO dto
        )
        {
            var entity = ServiceUsageMapper.ToEntity(dto);
            await _ServiceRepo.AddAsync(entity);
            await _ServiceRepo.SaveChangesAsync();

            return new GeneralResponse<ServiceUsageReadDTO>
            {
                Success = true,
                Message = "Service Usage created successfully.",
                Data = ServiceUsageMapper.ToReadDTO(entity),
            };
        }

        public async Task<GeneralResponse<ServiceUsageReadDTO>> GetByIdAsync(string id)
        {
            var entity = await _ServiceRepo.GetByIdAsync(id);
            return entity == null
                ? new()
                {
                    Success = false,
                    Message = "Service Usage not found.",
                    Data = null,
                }
                : new()
                {
                    Success = true,
                    Message = "Service Usage retrieved.",
                    Data = ServiceUsageMapper.ToReadDTO(entity),
                };
        }

        public async Task<GeneralResponse<IEnumerable<ServiceUsageReadDTO>>> GetAllAsync()
        {
            var entities = await _ServiceRepo.GetAllAsync();
            return new GeneralResponse<IEnumerable<ServiceUsageReadDTO>>
            {
                Success = true,
                Message = "All Service Usages retrieved.",
                Data = entities.Select(ServiceUsageMapper.ToReadDTO),
            };
        }

        public async Task<GeneralResponse<ServiceUsageReadDTO>> UpdateAsync(
            string id,
            ServiceUsageUpdateDTOs dto
        )
        {
            var entity = await _ServiceRepo.GetByIdAsync(id);
            if (entity == null)
                return new()
                {
                    Success = false,
                    Message = "Service Usage not found.",
                    Data = null,
                };

            ServiceUsageMapper.UpdateEntity(entity, dto);
            _ServiceRepo.Update(entity);
            await _ServiceRepo.SaveChangesAsync();

            return new GeneralResponse<ServiceUsageReadDTO>
            {
                Success = true,
                Message = "Service Usage updated.",
                Data = ServiceUsageMapper.ToReadDTO(entity),
            };
        }

        public async Task<GeneralResponse<string>> DeleteAsync(string id)
        {
            var entity = await _ServiceRepo.GetByIdAsync(id);
            if (entity == null)
                return new()
                {
                    Success = false,
                    Message = "Service Usage not found.",
                    Data = null,
                };

            _ServiceRepo.Remove(entity);
            await _ServiceRepo.SaveChangesAsync();

            return new GeneralResponse<string>
            {
                Success = true,
                Message = "Service Usage deleted.",
                Data = id,
            };
        }

        public async Task<GeneralResponse<ServiceUsageReadDTO>> TrackServiceUsageAsync(
            string customerId,
            ServiceType serviceType,
            string? techCompanyId = null
        )
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return new GeneralResponse<ServiceUsageReadDTO>
                {
                    Success = false,
                    Message = "Customer ID cannot be null or empty.",
                    Data = null,
                };
            }

            if (string.IsNullOrWhiteSpace(serviceType.ToString()))
            {
                return new GeneralResponse<ServiceUsageReadDTO>
                {
                    Success = false,
                    Message = "Service type cannot be null or empty.",
                    Data = null,
                };
            }

            try
            {
                var existingUsage = await _ServiceRepo.GetFirstOrDefaultAsync(
                    su => su.ServiceType.GetStringValue() == serviceType.ToString(),
                    includeProperties: "Maintenance,PCAssemblies,Orders"
                );

                if (existingUsage != null)
                {
                    existingUsage.CallCount++;
                    _ServiceRepo.Update(existingUsage);
                    await _ServiceRepo.SaveChangesAsync();

                    return new GeneralResponse<ServiceUsageReadDTO>
                    {
                        Success = true,
                        Message =
                            $"Service usage tracked successfully. Call count: {existingUsage.CallCount}",
                        Data = ServiceUsageMapper.ToReadDTO(existingUsage),
                    };
                }

                var newServiceUsage = new ServiceUsage
                {
                    Id = Guid.NewGuid().ToString(),
                    ServiceType = serviceType,
                    UsedOn = DateTime.Now,
                    CallCount = 1,
                };

                await _ServiceRepo.AddAsync(newServiceUsage);
                await _ServiceRepo.SaveChangesAsync();

                return new GeneralResponse<ServiceUsageReadDTO>
                {
                    Success = true,
                    Message = "New service usage created and tracked successfully.",
                    Data = ServiceUsageMapper.ToReadDTO(newServiceUsage),
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<ServiceUsageReadDTO>
                {
                    Success = false,
                    Message = $"An error occurred while tracking service usage: {ex.Message}",
                    Data = null,
                };
            }
        }

        public async Task<
            GeneralResponse<IEnumerable<ServiceUsageReadDTO>>
        > GetServiceUsageByCustomerAsync(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return new GeneralResponse<IEnumerable<ServiceUsageReadDTO>>
                {
                    Success = false,
                    Message = "Customer ID cannot be null or empty.",
                    Data = null,
                };
            }

            try
            {
                var serviceUsages = await _ServiceRepo.FindWithIncludesAsync(
                    su =>
                        su.Orders.Any(o => o.CustomerId == customerId)
                        || su.PCAssemblies.Any(pca => pca.CustomerId == customerId)
                        || su.Maintenance != null && su.Maintenance.CustomerId == customerId,
                    su => su.Orders,
                    su => su.PCAssemblies,
                    su => su.Maintenance
                );

                var serviceUsageDtos = serviceUsages
                    .Where(su => su != null)
                    .Select(ServiceUsageMapper.ToReadDTO)
                    .Where(dto => dto != null);

                return new GeneralResponse<IEnumerable<ServiceUsageReadDTO>>
                {
                    Success = true,
                    Message = "Customer service usage retrieved successfully.",
                    Data = serviceUsageDtos,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<ServiceUsageReadDTO>>
                {
                    Success = false,
                    Message =
                        $"An error occurred while retrieving customer service usage: {ex.Message}",
                    Data = null,
                };
            }
        }

        public async Task<
            GeneralResponse<IEnumerable<ServiceUsageReadDTO>>
        > GetServiceUsageByTechCompanyAsync(string techCompanyId)
        {
            if (string.IsNullOrWhiteSpace(techCompanyId))
            {
                return new GeneralResponse<IEnumerable<ServiceUsageReadDTO>>
                {
                    Success = false,
                    Message = "Tech Company ID cannot be null or empty.",
                    Data = null,
                };
            }

            try
            {
                var serviceUsages = await _ServiceRepo.FindWithIncludesAsync(
                    su => su.Maintenance != null && su.Maintenance.TechCompanyId == techCompanyId,
                    su => su.Maintenance,
                    su => su.PCAssemblies,
                    su => su.Orders
                );

                var serviceUsageDtos = serviceUsages
                    .Where(su => su != null)
                    .Select(ServiceUsageMapper.ToReadDTO)
                    .Where(dto => dto != null);

                return new GeneralResponse<IEnumerable<ServiceUsageReadDTO>>
                {
                    Success = true,
                    Message = "Tech Company service usage retrieved successfully.",
                    Data = serviceUsageDtos,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<ServiceUsageReadDTO>>
                {
                    Success = false,
                    Message =
                        $"An error occurred while retrieving tech company service usage: {ex.Message}",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<ServiceUsageReadDTO>> GetOrCreateServiceUsageAsync(
            string customerId,
            ServiceType serviceType
        )
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return new GeneralResponse<ServiceUsageReadDTO>
                {
                    Success = false,
                    Message = "Customer ID cannot be null or empty.",
                    Data = null,
                };
            }

            if (string.IsNullOrWhiteSpace(serviceType.ToString()))
            {
                return new GeneralResponse<ServiceUsageReadDTO>
                {
                    Success = false,
                    Message = "Service type cannot be null or empty.",
                    Data = null,
                };
            }

            try
            {
                var existingUsage = await _ServiceRepo.GetFirstOrDefaultAsync(
                    su => su.ServiceType == serviceType,
                    includeProperties: "Maintenance,PCAssemblies,Orders"
                );

                if (existingUsage != null)
                {
                    return new GeneralResponse<ServiceUsageReadDTO>
                    {
                        Success = true,
                        Message = "Existing service usage found.",
                        Data = ServiceUsageMapper.ToReadDTO(existingUsage),
                    };
                }

                var newServiceUsage = new ServiceUsage
                {
                    Id = Guid.NewGuid().ToString(),
                    ServiceType = serviceType,
                    UsedOn = DateTime.Now,
                    CallCount = 0,
                };

                await _ServiceRepo.AddAsync(newServiceUsage);
                await _ServiceRepo.SaveChangesAsync();

                return new GeneralResponse<ServiceUsageReadDTO>
                {
                    Success = true,
                    Message = "New service usage created successfully.",
                    Data = ServiceUsageMapper.ToReadDTO(newServiceUsage),
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<ServiceUsageReadDTO>
                {
                    Success = false,
                    Message =
                        $"An error occurred while getting or creating service usage: {ex.Message}",
                    Data = null,
                };
            }
        }
    }
}
