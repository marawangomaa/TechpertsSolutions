using Core.DTOs.ServiceUsage;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;

namespace Service
{
    public class ServiceUsageService : IServiceUsageService
    {
        private readonly IRepository<ServiceUsage> _ServiceRepo;

        public ServiceUsageService(IRepository<ServiceUsage> repo) => _ServiceRepo = repo;

        public async Task<GeneralResponse<ServiceUsageReadDTO>> CreateAsync(ServiceUsageCreateDTO dto)
        {
            var entity = ServiceUsageMapper.ToEntity(dto);
            await _ServiceRepo.AddAsync(entity);
            await _ServiceRepo.SaveChangesAsync();

            return new GeneralResponse<ServiceUsageReadDTO>
            {
                Success = true,
                Message = "Service Usage created successfully.",
                Data = ServiceUsageMapper.ToReadDTO(entity)
            };
        }

        public async Task<GeneralResponse<ServiceUsageReadDTO>> GetByIdAsync(string id)
        {
            var entity = await _ServiceRepo.GetByIdAsync(id);
            return entity == null
                ? new() { Success = false, Message = "Service Usage not found.", Data = null }
                : new() { Success = true, Message = "Service Usage retrieved.", Data = ServiceUsageMapper.ToReadDTO(entity) };
        }

        public async Task<GeneralResponse<IEnumerable<ServiceUsageReadDTO>>> GetAllAsync()
        {
            var entities = await _ServiceRepo.GetAllAsync();
            return new GeneralResponse<IEnumerable<ServiceUsageReadDTO>>
            {
                Success = true,
                Message = "All Service Usages retrieved.",
                Data = entities.Select(ServiceUsageMapper.ToReadDTO)
            };
        }

        public async Task<GeneralResponse<ServiceUsageReadDTO>> UpdateAsync(string id, ServiceUsageUpdateDTO dto)
        {
            var entity = await _ServiceRepo.GetByIdAsync(id);
            if (entity == null)
                return new() { Success = false, Message = "Service Usage not found.", Data = null };

            ServiceUsageMapper.UpdateEntity(entity, dto);
            _ServiceRepo.Update(entity);
            await _ServiceRepo.SaveChangesAsync();

            return new GeneralResponse<ServiceUsageReadDTO>
            {
                Success = true,
                Message = "Service Usage updated.",
                Data = ServiceUsageMapper.ToReadDTO(entity)
            };
        }

        public async Task<GeneralResponse<string>> DeleteAsync(string id)
        {
            var entity = await _ServiceRepo.GetByIdAsync(id);
            if (entity == null)
                return new() { Success = false, Message = "Service Usage not found.", Data = null };

            _ServiceRepo.Remove(entity);
            await _ServiceRepo.SaveChangesAsync();

            return new GeneralResponse<string>
            {
                Success = true,
                Message = "Service Usage deleted.",
                Data = id
            };
        }
    }
}
