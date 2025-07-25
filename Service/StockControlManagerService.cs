using Core.DTOs.StockControlManagerDTOs;
using Core.Interfaces;
using Core.Interfaces.Services;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class StockControlManagerService : IStockControlManagerService
    {
        private readonly IRepository<StockControlManager> _stockControlRepo;

        public StockControlManagerService(IRepository<StockControlManager> repo)
        {
            _stockControlRepo = repo;
        }

        public async Task<GeneralResponse<StockControlManagerReadDTO>> CreateAsync(StockControlManagerCreateDTO dto)
        {
            var entity = StockControlManagerMapper.ToEntity(dto);
            await _stockControlRepo.AddAsync(entity);
            await _stockControlRepo.SaveChangesAsync();

            return new GeneralResponse<StockControlManagerReadDTO>
            {
                Success = true,
                Message = "Stock Control Manager created successfully.",
                Data = StockControlManagerMapper.ToReadDTO(entity)
            };
        }

        public async Task<GeneralResponse<StockControlManagerReadDTO>> GetByIdAsync(string id)
        {
            var entity = await _stockControlRepo.GetByIdWithIncludesAsync(id, s => s.User, s => s.Role);
            if (entity == null)
            {
                return new GeneralResponse<StockControlManagerReadDTO>
                {
                    Success = false,
                    Message = "Stock Control Manager not found.",
                    Data = null
                };
            }

            return new GeneralResponse<StockControlManagerReadDTO>
            {
                Success = true,
                Message = "Retrieved successfully.",
                Data = StockControlManagerMapper.ToReadDTO(entity)
            };
        }

        public async Task<GeneralResponse<IEnumerable<StockControlManagerReadDTO>>> GetAllAsync()
        {
            var entities = await _stockControlRepo.GetAllWithIncludesAsync(s => s.User, s => s.Role);
            return new GeneralResponse<IEnumerable<StockControlManagerReadDTO>>
            {
                Success = true,
                Message = "All Stock Control Managers retrieved successfully.",
                Data = entities.Select(StockControlManagerMapper.ToReadDTO)
            };
        }

        public async Task<GeneralResponse<StockControlManagerReadDTO>> UpdateAsync(string id, StockControlManagerUpdateDTO dto)
        {
            var entity = await _stockControlRepo.GetByIdAsync(id);
            if (entity == null)
            {
                return new GeneralResponse<StockControlManagerReadDTO>
                {
                    Success = false,
                    Message = "Stock Control Manager not found.",
                    Data = null
                };
            }

            StockControlManagerMapper.UpdateEntity(entity, dto);
            _stockControlRepo.Update(entity);
            await _stockControlRepo.SaveChangesAsync();

            return new GeneralResponse<StockControlManagerReadDTO>
            {
                Success = true,
                Message = "Updated successfully.",
                Data = StockControlManagerMapper.ToReadDTO(entity)
            };
        }

        public async Task<GeneralResponse<string>> DeleteAsync(string id)
        {
            var entity = await _stockControlRepo.GetByIdAsync(id);
            if (entity == null)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "Stock Control Manager not found.",
                    Data = null
                };
            }

            _stockControlRepo.Remove(entity);
            await _stockControlRepo.SaveChangesAsync();

            return new GeneralResponse<string>
            {
                Success = true,
                Message = "Deleted successfully.",
                Data = id
            };
        }
    }
}
