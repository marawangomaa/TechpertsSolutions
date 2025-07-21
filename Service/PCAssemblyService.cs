using Core.DTOs.PCAssembly;
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
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class PCAssemblyService : IPCAssemblyService
    {
        private readonly IRepository<PCAssembly> _pcAssemblyRepo;

        public PCAssemblyService(IRepository<PCAssembly> pcAssemblyRepo)
        {
            _pcAssemblyRepo = pcAssemblyRepo;
        }

        public async Task<GeneralResponse<PCAssemblyReadDTO>> CreateAsync(PCAssemblyCreateDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.CustomerId))
            {
                return new GeneralResponse<PCAssemblyReadDTO>
                {
                    Success = false,
                    Message = "Invalid PC Assembly data.",
                    Data = null
                };
            }

            var pcAssembly = PCAssemblyMapper.ToEntity(dto);

            await _pcAssemblyRepo.AddAsync(pcAssembly);
            await _pcAssemblyRepo.SaveChangesAsync();

            return new GeneralResponse<PCAssemblyReadDTO>
            {
                Success = true,
                Message = "PC Assembly created successfully.",
                Data = PCAssemblyMapper.ToReadDTO(pcAssembly)
            };
        }

        public async Task<GeneralResponse<PCAssemblyReadDTO>> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<PCAssemblyReadDTO>
                {
                    Success = false,
                    Message = "ID cannot be null or empty.",
                    Data = null
                };
            }

            var assembly = await _pcAssemblyRepo.GetByIdWithIncludesAsync(id, a => a.PCAssemblyItems, a => a.Customer, a => a.ServiceUsage);

            if (assembly == null)
            {
                return new GeneralResponse<PCAssemblyReadDTO>
                {
                    Success = false,
                    Message = "PC Assembly not found.",
                    Data = null
                };
            }

            return new GeneralResponse<PCAssemblyReadDTO>
            {
                Success = true,
                Message = "PC Assembly retrieved successfully.",
                Data = PCAssemblyMapper.ToReadDTO(assembly)
            };
        }

        public async Task<GeneralResponse<IEnumerable<PCAssemblyReadDTO>>> GetAllAsync()
        {
            var assemblies = await _pcAssemblyRepo.GetAllWithIncludesAsync(a => a.PCAssemblyItems, a => a.Customer, a => a.ServiceUsage);

            return new GeneralResponse<IEnumerable<PCAssemblyReadDTO>>
            {
                Success = true,
                Message = "PC Assemblies retrieved successfully.",
                Data = assemblies.Select(PCAssemblyMapper.ToReadDTO)
            };
        }

        public async Task<GeneralResponse<IEnumerable<PCAssemblyReadDTO>>> GetByCustomerIdAsync(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return new GeneralResponse<IEnumerable<PCAssemblyReadDTO>>
                {
                    Success = false,
                    Message = "Customer ID cannot be null or empty.",
                    Data = null
                };
            }

            var assemblies = await _pcAssemblyRepo.FindWithIncludesAsync(a => a.CustomerId == customerId, a => a.PCAssemblyItems, a => a.Customer, a => a.ServiceUsage);

            return new GeneralResponse<IEnumerable<PCAssemblyReadDTO>>
            {
                Success = true,
                Message = "PC Assemblies for customer retrieved successfully.",
                Data = assemblies.Select(PCAssemblyMapper.ToReadDTO)
            };
        }
        public async Task<GeneralResponse<PCAssemblyReadDTO>> UpdatePCAssemblyAsync(string id, PCAssemblyUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(id) || dto == null)
            {
                return new GeneralResponse<PCAssemblyReadDTO>
                {
                    Success = false,
                    Message = "Invalid input.",
                    Data = null
                };
            }

            var assembly = await _pcAssemblyRepo.GetByIdWithIncludesAsync(id, a => a.PCAssemblyItems);

            if (assembly == null)
            {
                return new GeneralResponse<PCAssemblyReadDTO>
                {
                    Success = false,
                    Message = $"PC Assembly with ID '{id}' not found.",
                    Data = null
                };
            }

            PCAssemblyMapper.UpdateEntity(assembly, dto);

            if (dto.Items != null)
            {
                UpdatePCAssemblyItems(assembly, dto.Items);
            }

            _pcAssemblyRepo.Update(assembly);
            await _pcAssemblyRepo.SaveChangesAsync();

            return new GeneralResponse<PCAssemblyReadDTO>
            {
                Success = true,
                Message = "PC Assembly updated successfully.",
                Data = PCAssemblyMapper.ToReadDTO(assembly)
            };
        }
        private void UpdatePCAssemblyItems(PCAssembly entity, List<PCAssemblyItemUpdateDTO> updatedItems)
        {
            if (entity.PCAssemblyItems == null)
                entity.PCAssemblyItems = new List<PCAssemblyItem>();

            var dtoProvidedIds = updatedItems
                .Where(x => !string.IsNullOrWhiteSpace(x.Id))
                .Select(x => x.Id)
                .ToHashSet();

            entity.PCAssemblyItems.RemoveAll(i => i.Id != null && !dtoProvidedIds.Contains(i.Id));

            foreach (var dtoItem in updatedItems)
            {
                if (!string.IsNullOrWhiteSpace(dtoItem.Id))
                {
                    var existingItem = entity.PCAssemblyItems.FirstOrDefault(i => i.Id == dtoItem.Id);
                    if (existingItem != null)
                    {
                        existingItem.ProductId = dtoItem.ProductId;
                        existingItem.Quantity = dtoItem.Quantity;
                        existingItem.Price = dtoItem.Price;
                        existingItem.Total = dtoItem.Quantity * dtoItem.Price;
                    }
                }
                else
                {
                    entity.PCAssemblyItems.Add(new PCAssemblyItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductId = dtoItem.ProductId,
                        Quantity = dtoItem.Quantity,
                        Price = dtoItem.Price,
                        Total = dtoItem.Quantity * dtoItem.Price
                    });
                }
            }
        }
    }
}
