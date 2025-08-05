using Core.DTOs;
using Core.DTOs.CartDTOs;
using Core.DTOs.PCAssemblyDTOs;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Utilities;
using Microsoft.EntityFrameworkCore;
using Repository;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

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
                var assembly = await _pcAssemblyRepo.GetFirstOrDefaultAsync(
                                                                a => a.Id == id,
                                                                query => query
                                                                .Include(a => a.Customer)
                                                                .ThenInclude(c => c.User)
                                                                .Include(a => a.TechCompany)
                                                                .ThenInclude(tc => tc.User)
                                                                .Include(a => a.ServiceUsage)
                                                                .Include(a => a.PCAssemblyItems)
                                                                .ThenInclude(item => item.Product)
                                                                .ThenInclude(p => p.Category)
                                                                .Include(a => a.PCAssemblyItems)
                                                                .ThenInclude(item => item.Product)
                                                                .ThenInclude(p => p.SubCategory)
                                                                .Include(a => a.PCAssemblyItems)
                                                                .ThenInclude(item => item.Product)
                                                                 .ThenInclude(p => p.TechCompany));
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
                var assemblies = await _pcAssemblyRepo.GetAllAsync(query =>
                                              query.Include(a => a.Customer)
                                              .ThenInclude(c => c.User)
     .Include(a => a.TechCompany)
     .ThenInclude(tc => tc.User)
     .Include(a => a.ServiceUsage)
     .Include(a => a.PCAssemblyItems)
         .ThenInclude(item => item.Product)
             .ThenInclude(p => p.Category)
     .Include(a => a.PCAssemblyItems)
         .ThenInclude(item => item.Product)
             .ThenInclude(p => p.SubCategory)
     .Include(a => a.PCAssemblyItems)
         .ThenInclude(item => item.Product)
             .ThenInclude(p => p.TechCompany)
 );

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

            var assembly = await _pcAssemblyRepo.GetFirstOrDefaultAsync(a => a.Id == assemblyId, "PCAssemblyItems");
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
                TotalCost = assembly.PCAssemblyItems?.Sum(item => item.Total) ?? 0,
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

            var assembly = await _pcAssemblyRepo.GetFirstOrDefaultAsync(a => a.Id == assemblyId, "PCAssemblyItems");
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
                Subtotal = assembly.PCAssemblyItems?.Sum(item => item.Total) ?? 0,
                AssemblyFee = assembly.AssemblyFee ?? 0,
                TotalAmount = (assembly.PCAssemblyItems?.Sum(item => item.Total) ?? 0) + (assembly.AssemblyFee ?? 0)
            };

            return new GeneralResponse<PCBuildTotalDTO>
            {
                Success = true,
                Message = "PC Build total calculated successfully.",
                Data = total
            };
        }

        public async Task<GeneralResponse<IEnumerable<CompatibleComponentDTO>>> GetCompatibleProductsForCategoryAsync(string assemblyId, string categoryName)
        {
            // Step 1: Get assembly with items and their products/specs
            var assembly = await _pcAssemblyRepo.GetFirstOrDefaultAsync(
                a => a.Id == assemblyId,
                q => q.Include(a => a.PCAssemblyItems)
                      .ThenInclude(i => i.Product)
                          .ThenInclude(p => p.Specifications)
            );

            if (assembly == null)
            {
                return new GeneralResponse<IEnumerable<CompatibleComponentDTO>>
                {
                    Success = false,
                    Message = "PC Assembly not found.",
                    Data = null
                };
            }

            // Step 2: Aggregate specifications from already selected products
            var selectedSpecs = assembly.PCAssemblyItems
                .SelectMany(i => i.Product.Specifications)
                .GroupBy(s => s.Key)
                .ToDictionary(g => g.Key, g => g.First().Value);  // assumes same value for same key

            // Step 3: Get all products in the target category
            var productsInCategory = await _productRepo.FindAsync(
                p => p.Category.Name == categoryName,
                q => q.Include(p => p.Category)
                      .Include(p => p.Specifications)
            );

            // Step 4: Filter compatible products
            var compatibleProducts = productsInCategory
                .Where(p => AreSpecsCompatible(p.Specifications, selectedSpecs))
                .Select(p => new CompatibleComponentDTO
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    Price = p.Price,
                    Category = p.Category?.Name,
                    CompatibilityScore = CalculateCompatibilityScore(selectedSpecs, p.Specifications)
                });

            return new GeneralResponse<IEnumerable<CompatibleComponentDTO>>
            {
                Success = true,
                Message = "Compatible products retrieved successfully.",
                Data = compatibleProducts
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
        public async Task<GeneralResponse<IEnumerable<PCAssemblyItemReadDTO>>> GetAllComponentsAsync(string assemblyId)
        {
            if (string.IsNullOrWhiteSpace(assemblyId))
            {
                return new GeneralResponse<IEnumerable<PCAssemblyItemReadDTO>>
                {
                    Success = false,
                    Message = "Assembly ID cannot be null or empty.",
                    Data = null
                };
            }

            var assembly = await _pcAssemblyRepo.GetFirstOrDefaultAsync(a => a.Id == assemblyId, 
                "PCAssemblyItems,PCAssemblyItems.Product,PCAssemblyItems.Product.Category,PCAssemblyItems.Product.SubCategory,PCAssemblyItems.Product.TechCompany");

            if (assembly == null)
            {
                return new GeneralResponse<IEnumerable<PCAssemblyItemReadDTO>>
                {
                    Success = false,
                    Message = "PC Assembly not found.",
                    Data = null
                };
            }

            var components = assembly.PCAssemblyItems?.Select(item => new PCAssemblyItemReadDTO
            {
                ItemId = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product?.Name ?? string.Empty,
                SubCategoryName = item.Product?.SubCategory?.Name,
                Category = item.Product?.Category?.Name ?? string.Empty,
                Status = item.Product?.status.ToString() ?? string.Empty,
                Price = item.Price,
                Discount = item.Product?.DiscountPrice,
                Quantity = item.Quantity,
                Total = item.Total
            }) ?? new List<PCAssemblyItemReadDTO>();

            return new GeneralResponse<IEnumerable<PCAssemblyItemReadDTO>>
            {
                Success = true,
                Message = "Components retrieved successfully.",
                Data = components
            };
        }

        public async Task<PCAssemblyItemReadDTO?> GetComponentByCategoryAsync(string assemblyId, ProductCategory category)
        {
            if (string.IsNullOrWhiteSpace(assemblyId))
                return null;

            var assembly = await _pcAssemblyRepo.GetFirstOrDefaultAsync(a => a.Id == assemblyId,
                "PCAssemblyItems,PCAssemblyItems.Product,PCAssemblyItems.Product.Category,PCAssemblyItems.Product.SubCategory");

            if (assembly?.PCAssemblyItems == null)
                return null;

            var item = assembly.PCAssemblyItems.FirstOrDefault(pi => pi.Product?.Category?.Name == category.ToString());
            if (item == null) return null;

            return new PCAssemblyItemReadDTO
            {
                ItemId = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product?.Name ?? string.Empty,
                SubCategoryName = item.Product?.SubCategory?.Name,
                Category = item.Product?.Category?.Name ?? string.Empty,
                Status = item.Product?.status.ToString() ?? string.Empty,
                Price = item.Price,
                Discount = item.Product?.DiscountPrice,
                Quantity = item.Quantity,
                Total = item.Total
            };
        }

        public async Task<GeneralResponse<CartReadDTO>> SaveBuildToCartAsync(string assemblyId, string customerId, decimal assemblyFee)
        {
            if (string.IsNullOrWhiteSpace(assemblyId) || string.IsNullOrWhiteSpace(customerId))
            {
                return new GeneralResponse<CartReadDTO>
                {
                    Success = false,
                    Message = "Assembly ID and Customer ID cannot be null or empty.",
                    Data = null
                };
            }

            try
            {
                // Get the assembly with all components
                var assembly = await _pcAssemblyRepo.GetFirstOrDefaultAsync(a => a.Id == assemblyId,
                    "PCAssemblyItems,PCAssemblyItems.Product");

                if (assembly == null)
                {
                    return new GeneralResponse<CartReadDTO>
                    {
                        Success = false,
                        Message = "PC Assembly not found.",
                        Data = null
                    };
                }

                // Calculate total including assembly fee
                var componentsTotal = assembly.PCAssemblyItems?.Sum(item => item.Total) ?? 0;
                var totalAmount = componentsTotal + assemblyFee;

                // Update assembly with fee and mark as completed
                assembly.AssemblyFee = assemblyFee;
                assembly.Status = PCAssemblyStatus.Completed;
                assembly.CompletedDate = DateTime.UtcNow;

                _pcAssemblyRepo.Update(assembly);
                await _pcAssemblyRepo.SaveChangesAsync();

                // Create a special cart item for the PC build
                var pcBuildItem = new CartItemDTO
                {
                    ProductId = assemblyId, // Using assembly ID as product ID for PC builds
                    Quantity = 1,
                    UnitPrice = totalAmount,
                    TotalPrice = totalAmount
                };

                // Add to cart (this would need to be implemented in cart service)
                // For now, return success with the calculated total
                return new GeneralResponse<CartReadDTO>
                {
                    Success = true,
                    Message = "PC Build saved to cart successfully.",
                    Data = new CartReadDTO
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomerId = customerId,
                        CreatedAt = DateTime.UtcNow,
                        SubTotal = totalAmount,
                        CartItems = new List<CartItemReadDTO>()
                    }
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<CartReadDTO>
                {
                    Success = false,
                    Message = "Failed to save PC build to cart.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<PCBuildTableDTO>> GetPCBuildTableAsync(string assemblyId)
        {
            if (string.IsNullOrWhiteSpace(assemblyId))
            {
                return new GeneralResponse<PCBuildTableDTO>
                {
                    Success = false,
                    Message = "Assembly ID cannot be null or empty.",
                    Data = null
                };
            }

            try
            {
                var assembly = await _pcAssemblyRepo.GetFirstOrDefaultAsync(a => a.Id == assemblyId,
                    "PCAssemblyItems,PCAssemblyItems.Product,PCAssemblyItems.Product.Category,PCAssemblyItems.Product.SubCategory");

                if (assembly == null)
                {
                    return new GeneralResponse<PCBuildTableDTO>
                    {
                        Success = false,
                        Message = "PC Assembly not found.",
                        Data = null
                    };
                }

                // Define all PC component categories
                var allCategories = new[]
                {
                    ProductCategory.Processor,
                    ProductCategory.Motherboard,
                    ProductCategory.CPUCooler,
                    ProductCategory.Case,
                    ProductCategory.GraphicsCard,
                    ProductCategory.RAM,
                    ProductCategory.Storage,
                    ProductCategory.CaseCooler,
                    ProductCategory.PowerSupply,
                    ProductCategory.Monitor,
                    ProductCategory.Accessories
                };

                var components = new List<PCBuildTableItemDTO>();
                var totalCost = 0m;

                foreach (var category in allCategories)
                {
                    var existingItem = assembly.PCAssemblyItems?.FirstOrDefault(pi => 
                        pi.Product?.Category?.Name == category.ToString());

                    var component = new PCBuildTableItemDTO
                    {
                        ComponentType = category,
                        ComponentDisplayName = GetComponentDisplayName(category),
                        HasComponent = existingItem != null
                    };

                    if (existingItem != null)
                    {
                        component.ProductId = existingItem.ProductId;
                        component.ProductName = existingItem.Product?.Name;
                        component.SubCategoryName = existingItem.Product?.SubCategory?.Name;
                        component.Status = "Selected";
                        component.Price = existingItem.Price;
                        component.Discount = existingItem.Product?.DiscountPrice;
                        component.ItemId = existingItem.Id;
                        totalCost += existingItem.Total;
                    }
                    else
                    {
                        component.Status = null;
                    }

                    components.Add(component);
                }

                var buildTable = new PCBuildTableDTO
                {
                    AssemblyId = assembly.Id,
                    Components = components,
                    TotalCost = totalCost,
                    AssemblyFee = assembly.AssemblyFee ?? 0,
                    GrandTotal = totalCost + (assembly.AssemblyFee ?? 0),
                    IsComplete = assembly.Status == PCAssemblyStatus.Completed
                };

                return new GeneralResponse<PCBuildTableDTO>
                {
                    Success = true,
                    Message = "PC Build table retrieved successfully.",
                    Data = buildTable
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PCBuildTableDTO>
                {
                    Success = false,
                    Message = "Failed to retrieve PC build table.",
                    Data = null
                };
            }
        }

        private string GetComponentDisplayName(ProductCategory category)
        {
            return category switch
            {
                ProductCategory.Processor => "Processor",
                ProductCategory.Motherboard => "Motherboard",
                ProductCategory.CPUCooler => "CPU Cooler",
                ProductCategory.Case => "Case",
                ProductCategory.GraphicsCard => "Graphics Card",
                ProductCategory.RAM => "RAM",
                ProductCategory.Storage => "Storage",
                ProductCategory.CaseCooler => "Case Cooler",
                ProductCategory.PowerSupply => "Power Supply",
                ProductCategory.Monitor => "Monitor",
                ProductCategory.Accessories => "Accessories",
                _ => category.ToString()
            };
        }
        private bool AreSpecsCompatible(IEnumerable<Specification> candidateSpecs, Dictionary<string, string> selectedSpecs)
        {
            foreach (var key in selectedSpecs.Keys)
            {
                var candidate = candidateSpecs.FirstOrDefault(s => s.Key == key);
                if (candidate == null || candidate.Value != selectedSpecs[key])
                    return false; // mismatch
            }
            return true;
        }
        private decimal CalculateCompatibilityScore(Dictionary<string, string> selectedSpecs, IEnumerable<Specification> candidateSpecs)
        {
            int matched = 0;
            int total = selectedSpecs.Count;

            foreach (var spec in candidateSpecs)
            {
                if (selectedSpecs.TryGetValue(spec.Key, out var selectedValue) && selectedValue == spec.Value)
                {
                    matched++;
                }
            }

            return total == 0 ? 0 : (decimal)matched / total;
        }
    }
}
