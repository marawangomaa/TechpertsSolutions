using Core.DTOs.PCAssemblyDTOs;
using Core.DTOs.ProductDTOs;
using Core.Entities;
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
using TechpertsSolutions.Core.DTOs;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class PCAssemblyService : IPCAssemblyService
    {
        private readonly IRepository<PCAssembly> _pcAssemblyRepo;
        private readonly IRepository<Product> _productRepo;

        public PCAssemblyService(IRepository<PCAssembly> pcAssemblyRepo, IRepository<Product> productRepo)
        {
            _pcAssemblyRepo = pcAssemblyRepo;
            _productRepo = productRepo;
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

            var assembly = await _pcAssemblyRepo.GetByIdWithIncludesAsync(assemblyId, a => a.PCAssemblyItems);
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

            
            var existingItem = assembly.PCAssemblyItems?.FirstOrDefault(i => 
                i.ProductId == item.ProductId);

            
            if (existingItem == null && product.Category != null)
            {
                var existingItems = assembly.PCAssemblyItems?.ToList() ?? new List<PCAssemblyItem>();
                foreach (var existingAssemblyItem in existingItems)
                {
                    var existingProduct = await _productRepo.GetByIdAsync(existingAssemblyItem.ProductId);
                    if (existingProduct?.Category?.Name == product.Category.Name)
                    {
                        existingItem = existingAssemblyItem;
                        break;
                    }
                }
            }

            if (existingItem != null)
            {
                assembly.PCAssemblyItems.Remove(existingItem);
            }

            
            var newItem = new PCAssemblyItem
            {
                Id = Guid.NewGuid().ToString(),
                PCAssemblyId = assemblyId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Price,
                Total = item.Quantity * item.Price
            };

            if (assembly.PCAssemblyItems == null)
                assembly.PCAssemblyItems = new List<PCAssemblyItem>();

            assembly.PCAssemblyItems.Add(newItem);

            _pcAssemblyRepo.Update(assembly);
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

            var assembly = await _pcAssemblyRepo.GetByIdWithIncludesAsync(assemblyId, a => a.PCAssemblyItems);
            if (assembly == null)
            {
                return new GeneralResponse<PCAssemblyReadDTO>
                {
                    Success = false,
                    Message = "PC Assembly not found.",
                    Data = null
                };
            }

            var item = assembly.PCAssemblyItems?.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
            {
                return new GeneralResponse<PCAssemblyReadDTO>
                {
                    Success = false,
                    Message = "Component not found in PC build.",
                    Data = null
                };
            }

            assembly.PCAssemblyItems.Remove(item);
            _pcAssemblyRepo.Update(assembly);
            await _pcAssemblyRepo.SaveChangesAsync();

            return new GeneralResponse<PCAssemblyReadDTO>
            {
                Success = true,
                Message = "Component removed from PC build successfully.",
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

            var assembly = await _pcAssemblyRepo.GetByIdWithIncludesAsync(assemblyId, 
                a => a.PCAssemblyItems, a => a.Customer, a => a.ServiceUsage);

            if (assembly == null)
            {
                return new GeneralResponse<PCBuildStatusDTO>
                {
                    Success = false,
                    Message = "PC Assembly not found.",
                    Data = null
                };
            }

            var buildStatus = new PCBuildStatusDTO
            {
                AssemblyId = assembly.Id,
                CustomerId = assembly.CustomerId,
                Name = assembly.Name,
                CreatedAt = assembly.CreatedAt,
                Components = new List<PCBuildComponentDTO>(),
                ComponentStatus = new Dictionary<string, bool>()
            };

            
            var allCategories = Enum.GetValues(typeof(ProductCategory))
                .Cast<ProductCategory>()
                .Where(c => c != ProductCategory.PreBuildPC && c != ProductCategory.Laptop)
                .ToList();

            
            foreach (var category in allCategories)
            {
                buildStatus.ComponentStatus[category.ToString()] = false;
            }

            
            if (assembly.PCAssemblyItems != null)
            {
                foreach (var item in assembly.PCAssemblyItems)
                {
                    var product = await _productRepo.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        buildStatus.Components.Add(new PCBuildComponentDTO
                        {
                            Id = item.Id,
                            ProductId = item.ProductId,
                            ProductName = product.Name,
                            ProductImage = product.ImageUrl,
                            Category = product.Category?.Name ?? "Unknown",
                            SubCategory = product.SubCategory?.Name ?? "Unknown",
                            Price = item.Price,
                            DiscountPrice = product.DiscountPrice,
                            Quantity = item.Quantity,
                            Total = item.Total,
                            IsSelected = true
                        });

                        if (product.Category?.Name != null)
                        {
                            try
                            {
                                var categoryEnum = EnumExtensions.ParseFromStringValue<ProductCategory>(product.Category.Name);
                                buildStatus.ComponentStatus[categoryEnum.ToString()] = true;
                            }
                            catch
                            {
                                
                            }
                        }
                    }
                }
            }

            buildStatus.TotalPrice = buildStatus.Components.Sum(c => c.Total);

            return new GeneralResponse<PCBuildStatusDTO>
            {
                Success = true,
                Message = "PC Build status retrieved successfully.",
                Data = buildStatus
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

            var subtotal = assembly.PCAssemblyItems?.Sum(i => i.Total) ?? 0;
            var discount = 0m; 
            var total = subtotal - discount;

            var buildTotal = new PCBuildTotalDTO
            {
                AssemblyId = assemblyId,
                Subtotal = subtotal,
                Discount = discount,
                Total = total,
                TotalComponents = assembly.PCAssemblyItems?.Count ?? 0,
                SelectedComponents = assembly.PCAssemblyItems?.Count ?? 0
            };

            return new GeneralResponse<PCBuildTotalDTO>
            {
                Success = true,
                Message = "Build total calculated successfully.",
                Data = buildTotal
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

            var baseProduct = await _productRepo.GetByIdAsync(productId);
            if (baseProduct == null)
            {
                return new GeneralResponse<IEnumerable<CompatibleComponentDTO>>
                {
                    Success = false,
                    Message = "Base product not found.",
                    Data = null
                };
            }

            
            var compatibleProducts = await _productRepo.FindAsync(p => 
                p.Id != productId && 
                p.CategoryId == baseProduct.CategoryId);

            var compatibleComponents = compatibleProducts.Select(p => new CompatibleComponentDTO
            {
                ProductId = p.Id,
                ProductName = p.Name,
                ProductImage = p.ImageUrl,
                Category = p.Category?.Name ?? "Unknown",
                Price = p.Price,
                DiscountPrice = p.DiscountPrice,
                CompatibilityReason = "Same category component",
                CompatibilityScore = 0.8 
            }).ToList();

            return new GeneralResponse<IEnumerable<CompatibleComponentDTO>>
            {
                Success = true,
                Message = "Compatible components retrieved successfully.",
                Data = compatibleComponents
            };
        }
    }
}
