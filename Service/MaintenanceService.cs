using Core.DTOs.Maintenance;
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

        public async Task<IEnumerable<MaintenanceDTO>> GetAllAsync()
        {
            var maintenances = await _maintenanceRepo.GetAllAsync();
            return MaintenanceMapper.MapToMaintenanceDTOList(maintenances);
        }

        public async Task<MaintenanceDetailsDTO?> GetByIdAsync(string id)
        {
            var maintenance = await _maintenanceRepo.GetByIdAsync(id);
            return MaintenanceMapper.MapToMaintenanceDetailsDTO(maintenance);
        }

        public async Task<MaintenanceDTO> AddAsync(MaintenanceCreateDTO dto)
        {
            var entity = MaintenanceMapper.MapToMaintenance(dto);

            await _maintenanceRepo.AddAsync(entity);
            await _maintenanceRepo.SaveChangesAsync();

            var customer = await _customerRepo.GetByIdAsync(dto.CustomerId);
            var techCompany = await _techCompanyRepo.GetByIdAsync(dto.TechCompanyId);
            var warranty = await _warrantyRepo.GetByIdAsync(dto.WarrantyId);
            var serviceUsage = await _serviceUsageRepo.GetByIdAsync(dto.ServiceUsageId);

            return MaintenanceMapper.MapToMaintenanceDTO(entity, 
                customer?.User?.FullName ?? "Unknown",
                techCompany?.User?.FullName ?? "Unknown",
                warranty?.Product?.Name ?? "Unknown",
                serviceUsage?.ServiceType ?? "Unknown",
                warranty?.StartDate ?? DateTime.MinValue,
                warranty?.EndDate ?? DateTime.MinValue);
        }


        //public async Task<bool> UpdateAsync(string id, MaintenanceUpdateDTO dto)
        //{
        //    if (string.IsNullOrWhiteSpace(id)) return false;

        //    var maintenance = await _maintenanceRepo.GetByIdAsync(id);
        //    if (maintenance == null) return false;

        //    if (await _customerRepo.GetByIdAsync(dto.CustomerId) == null) return false;
        //    if (await _techCompanyRepo.GetByIdAsync(dto.TechCompanyId) == null) return false;
        //    if (await _warrantyRepo.GetByIdAsync(dto.WarrantyId) == null) return false;
        //    if (await _serviceUsageRepo.GetByIdAsync(dto.ServiceUsageId) == null) return false;

        //    maintenance.CustomerId = dto.CustomerId;
        //    maintenance.TechCompanyId = dto.TechCompanyId;
        //    maintenance.WarrantyId = dto.WarrantyId;
        //    maintenance.ServiceUsageId = dto.ServiceUsageId;

        //    _maintenanceRepo.Update(maintenance);
        //    await _maintenanceRepo.SaveChangesAsync();
        //    return true;
        //}
        public async Task<bool> UpdateAsync(string id, MaintenanceUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;

            var maintenance = await _maintenanceRepo.GetByIdAsync(id);
            if (maintenance == null) return false;

            var customerTask = _customerRepo.GetByIdAsync(dto.CustomerId);
            var techCompanyTask = _techCompanyRepo.GetByIdAsync(dto.TechCompanyId);
            var warrantyTask = _warrantyRepo.GetByIdAsync(dto.WarrantyId);
            var serviceUsageTask = _serviceUsageRepo.GetByIdAsync(dto.ServiceUsageId);

            await Task.WhenAll(customerTask, techCompanyTask, warrantyTask, serviceUsageTask);

            if (customerTask.Result == null || techCompanyTask.Result == null ||
                warrantyTask.Result == null || serviceUsageTask.Result == null)
            {
                return false;
            }

            MaintenanceMapper.MapToMaintenance(dto, maintenance);

            _maintenanceRepo.Update(maintenance);
            await _maintenanceRepo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await _maintenanceRepo.GetByIdAsync(id);
            if (entity == null)
                return false;

            _maintenanceRepo.Remove(entity);
            await _maintenanceRepo.SaveChangesAsync();
            return true;
        }


    }
}
