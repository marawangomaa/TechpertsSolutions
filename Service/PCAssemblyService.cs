using Core.DTOs.PCAssemblyDTOs;
using Core.DTOs;
using TechpertsSolutions.Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Utilities;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class PCAssemblyService : IPCAssemblyService
    {
        private readonly IRepository<PCAssembly> _pcAssemblyRepo;
        private readonly IRepository<PCAssemblyItem> _pcAssemblyItemRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Customer> _customerRepo;
        private readonly IRepository<ServiceUsage> _serviceUsageRepo;
        private readonly INotificationService _notificationService;

        public PCAssemblyService(
            IRepository<PCAssembly> pcAssemblyRepo,
            IRepository<PCAssemblyItem> pcAssemblyItemRepo,
            IRepository<Product> productRepo,
            IRepository<Customer> customerRepo,
            IRepository<ServiceUsage> serviceUsageRepo,
            INotificationService notificationService)
        {
            _pcAssemblyRepo = pcAssemblyRepo;
            _pcAssemblyItemRepo = pcAssemblyItemRepo;
            _productRepo = productRepo;
            _customerRepo = customerRepo;
            _serviceUsageRepo = serviceUsageRepo;
            _notificationService = notificationService;
        }

        public async Task<GeneralResponse<PCAssemblyReadDTO>> CreateAsync(PCAssemblyCreateDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.CustomerId))
            {
                return new GeneralResponse<PCAssemblyReadDTO>
                {
                    Success = false,
                    Message = "Invalid input data.",
                    Data = null
                };
            }

            var pcAssembly = new PCAssembly
            {
                Name = dto.Name,
                CustomerId = dto.CustomerId,
                ServiceUsageId = dto.ServiceUsageId,
                Description = dto.Description,
                Budget = dto.Budget,
                Status = PCAssemblyStatus.Requested
            };

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
                    Message = "PC Assembly ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<PCAssemblyReadDTO>
                {
                    Success = false,
                    Message = "Invalid PC Assembly ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                // Comprehensive includes for detailed PC assembly view
                var assembly = await _pcAssemblyRepo.GetByIdWithIncludesAsync(id, 
                    a => a.Customer,
                    a => a.Customer.User,
                    a => a.TechCompany,
                    a => a.TechCompany.User,
                    a => a.ServiceUsage,
                    a => a.PCAssemblyItems,
                    a => a.PCAssemblyItems.Select(pi => pi.Product),
                    a => a.PCAssemblyItems.Select(pi => pi.Product.Category),
                    a => a.PCAssemblyItems.Select(pi => pi.Product.SubCategory),
                    a => a.PCAssemblyItems.Select(pi => pi.Product.TechCompany));

                if (assembly == null)
                {
                    return new GeneralResponse<PCAssemblyReadDTO>
                    {
                        Success = false,
                        Message = $"PC Assembly with ID '{id}' not found.",
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
            catch (Exception ex)
            {
                return new GeneralResponse<PCAssemblyReadDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the PC Assembly.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<PCAssemblyReadDTO>>> GetAllAsync()
        {
            try
            {
                // Optimized includes for PC assembly listing with essential related data
                var assemblies = await _pcAssemblyRepo.GetAllWithIncludesAsync(
                    a => a.Customer,
                    a => a.Customer.User,
                    a => a.TechCompany,
                    a => a.TechCompany.User,
                    a => a.ServiceUsage,
                    a => a.PCAssemblyItems);

                var assemblyDtos = assemblies.Select(PCAssemblyMapper.ToReadDTO).ToList();

                return new GeneralResponse<IEnumerable<PCAssemblyReadDTO>>
                {
                    Success = true,
                    Message = "PC Assemblies retrieved successfully.",
                    Data = assemblyDtos
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<PCAssemblyReadDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving PC Assemblies.",
                    Data = null
                };
            }
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

            var assemblies = await _pcAssemblyRepo.FindWithStringIncludesAsync(a => a.CustomerId == customerId, includeProperties: "PCAssemblyItems,PCAssemblyItems.Product,Customer,ServiceUsage");

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
                    Message = "Invalid input data.",
                    Data = null
                };
            }

            var assembly = await _pcAssemblyRepo.GetByIdAsync(id);
            if (assembly == null)
            {
                return new GeneralResponse<PCAssemblyReadDTO>
                {
                    Success = false,
                    Message = "PC Assembly not found.",
                    Data = null
                };
            }

            assembly.Name = dto.Name;
            assembly.Description = dto.Description;
            assembly.Budget = dto.Budget;
            if (dto.Status.HasValue)
                assembly.Status = dto.Status.Value;

            _pcAssemblyRepo.Update(assembly);
            await _pcAssemblyRepo.SaveChangesAsync();

                            return new GeneralResponse<PCAssemblyReadDTO>
                {
                    Success = true,
                    Message = "PC Assembly updated successfully.",
                    Data = PCAssemblyMapper.ToReadDTO(assembly)
                };
        }

        public async Task<GeneralResponse<PCBuildStatusDTO>> GetPCBuildStatusAsync(string assemblyId)
        {
            if (string.IsNullOrWhiteSpace(assemblyId))
            {
                return new GeneralResponse<PCBuildStatusDTO>
                {
                    Success = false,
                    Message = "Assembly ID cannot be null or empty.",
                    Data = null
                };
            }

            var assembly = await _pcAssemblyRepo.GetByIdWithIncludesAsync(assemblyId, a => a.PCAssemblyItems);
            if (assembly == null)
            {
                return new GeneralResponse<PCBuildStatusDTO>
                {
                    Success = false,
                    Message = "PC Assembly not found.",
                    Data = null
                };
            }

            var status = new PCBuildStatusDTO
            {
                AssemblyId = assembly.Id,
                Status = assembly.Status.ToString(),
                ComponentCount = assembly.PCAssemblyItems?.Count ?? 0,
                TotalCost = assembly.PCAssemblyItems?.Sum(item => item.Quantity * item.UnitPrice) ?? 0,
                IsComplete = assembly.Status == PCAssemblyStatus.Completed
            };

            return new GeneralResponse<PCBuildStatusDTO>
            {
                Success = true,
                Message = "PC Build status retrieved successfully.",
                Data = status
            };
        }

        public async Task<GeneralResponse<PCBuildTotalDTO>> CalculateBuildTotalAsync(string assemblyId)
        {
            if (string.IsNullOrWhiteSpace(assemblyId))
            {
                return new GeneralResponse<PCBuildTotalDTO>
                {
                    Success = false,
                    Message = "Assembly ID cannot be null or empty.",
                    Data = null
                };
            }

            var assembly = await _pcAssemblyRepo.GetByIdWithIncludesAsync(assemblyId, a => a.PCAssemblyItems);
            if (assembly == null)
            {
                return new GeneralResponse<PCBuildTotalDTO>
                {
                    Success = false,
                    Message = "PC Assembly not found.",
                    Data = null
                };
            }

            var total = new PCBuildTotalDTO
            {
                AssemblyId = assembly.Id,
                Subtotal = assembly.PCAssemblyItems?.Sum(item => item.Quantity * item.UnitPrice) ?? 0,
                AssemblyFee = assembly.AssemblyFee ?? 0,
                TotalAmount = (assembly.PCAssemblyItems?.Sum(item => item.Quantity * item.UnitPrice) ?? 0) + (assembly.AssemblyFee ?? 0)
            };

            return new GeneralResponse<PCBuildTotalDTO>
            {
                Success = true,
                Message = "PC Build total calculated successfully.",
                Data = total
            };
        }

        public async Task<GeneralResponse<IEnumerable<CompatibleComponentDTO>>> GetCompatibleComponentsAsync(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                return new GeneralResponse<IEnumerable<CompatibleComponentDTO>>
                {
                    Success = false,
                    Message = "Product ID cannot be null or empty.",
                    Data = null
                };
            }

            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null)
            {
                return new GeneralResponse<IEnumerable<CompatibleComponentDTO>>
                {
                    Success = false,
                    Message = "Product not found.",
                    Data = null
                };
            }

            // This is a simplified compatibility check - in a real application, you'd have more complex logic
            var compatibleProducts = await _productRepo.FindAsync(p => p.CategoryId == product.CategoryId && p.Id != productId);

            var compatibleComponents = compatibleProducts.Select(p => new CompatibleComponentDTO
            {
                ProductId = p.Id,
                ProductName = p.Name,
                Price = p.Price,
                Category = p.Category?.Name,
                CompatibilityScore = 0.8m // Simplified score
            });

            return new GeneralResponse<IEnumerable<CompatibleComponentDTO>>
            {
                Success = true,
                Message = "Compatible components retrieved successfully.",
                Data = compatibleComponents
            };
        }

        public async Task<GeneralResponse<PCAssemblyReadDTO>> AddComponentToAssemblyAsync(string assemblyId, PCAssemblyItemCreateDTO item)
        {
            if (string.IsNullOrWhiteSpace(assemblyId) || item == null)
            {
                return new GeneralResponse<PCAssemblyReadDTO>
                {
                    Success = false,
                    Message = "Invalid input data.",
                    Data = null
                };
            }

            var assembly = await _pcAssemblyRepo.GetByIdAsync(assemblyId);
            if (assembly == null)
            {
                return new GeneralResponse<PCAssemblyReadDTO>
                {
                    Success = false,
                    Message = "PC Assembly not found.",
                    Data = null
                };
            }

            var product = await _productRepo.GetByIdAsync(item.ProductId);
            if (product == null)
            {
                return new GeneralResponse<PCAssemblyReadDTO>
                {
                    Success = false,
                    Message = "Product not found.",
                    Data = null
                };
            }

            var assemblyItem = new PCAssemblyItem
            {
                PCAssemblyId = assemblyId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = product.Price,
                UnitPrice = product.Price,
                Total = item.Quantity * product.Price
            };

            await _pcAssemblyItemRepo.AddAsync(assemblyItem);
            await _pcAssemblyRepo.SaveChangesAsync();

            return new GeneralResponse<PCAssemblyReadDTO>
            {
                Success = true,
                Message = "Component added to PC build successfully.",
                Data = PCAssemblyMapper.ToReadDTO(assembly)
            };
        }

        public async Task<GeneralResponse<PCAssemblyReadDTO>> RemoveComponentFromAssemblyAsync(string assemblyId, string itemId)
        {
            if (string.IsNullOrWhiteSpace(assemblyId) || string.IsNullOrWhiteSpace(itemId))
            {
                return new GeneralResponse<PCAssemblyReadDTO>
                {
                    Success = false,
                    Message = "Invalid input data.",
                    Data = null
                };
            }

            var assembly = await _pcAssemblyRepo.GetByIdAsync(assemblyId);
            if (assembly == null)
            {
                return new GeneralResponse<PCAssemblyReadDTO>
                {
                    Success = false,
                    Message = "PC Assembly not found.",
                    Data = null
                };
            }

            var item = await _pcAssemblyItemRepo.GetByIdAsync(itemId);
            if (item == null)
            {
                return new GeneralResponse<PCAssemblyReadDTO>
                {
                    Success = false,
                    Message = "Assembly item not found.",
                    Data = null
                };
            }

            _pcAssemblyItemRepo.Remove(item);
            await _pcAssemblyRepo.SaveChangesAsync();

            return new GeneralResponse<PCAssemblyReadDTO>
            {
                Success = true,
                Message = "Component removed from PC build successfully.",
                Data = PCAssemblyMapper.ToReadDTO(assembly)
            };
        }
    }
}
