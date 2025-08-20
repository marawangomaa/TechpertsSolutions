using Core.DTOs;
using Core.DTOs.ProductDTOs;
using Core.DTOs.ProfileDTOs;
using Core.Interfaces;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IRepository<Customer> _customerRepo;
        private readonly IRepository<TechCompany> _techCompanyRepo;
        private readonly IRepository<DeliveryPerson> _deliveryPersonRepo;

        public ProfileService(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IRepository<Customer> customerRepo,
            IRepository<TechCompany> techCompanyRepo,
            IRepository<DeliveryPerson> deliveryPersonRepo
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _customerRepo = customerRepo;
            _techCompanyRepo = techCompanyRepo;
            _deliveryPersonRepo = deliveryPersonRepo;
        }

        public async Task<GeneralResponse<IEnumerable<GeneralProfileReadDTO>>> GetAllProfilesAsync()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();

                var profileDtos = new List<GeneralProfileReadDTO>();
                foreach (var u in users)
                {
                    var roles = await _userManager.GetRolesAsync(u);
                    profileDtos.Add(MapToProfileDTO(u, roles));
                }

                return new GeneralResponse<IEnumerable<GeneralProfileReadDTO>>
                {
                    Success = true,
                    Message = "Profiles retrieved successfully.",
                    Data = profileDtos,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<GeneralProfileReadDTO>>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving profiles: {ex.Message}",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<GeneralProfileReadDTO>> GetProfileByIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return new GeneralResponse<GeneralProfileReadDTO>
                    {
                        Success = false,
                        Message = "User not found",
                        Data = null,
                    };
                }

                var roles = await _userManager.GetRolesAsync(user);
                var dto = MapToProfileDTO(user, roles);

                return new GeneralResponse<GeneralProfileReadDTO>
                {
                    Success = true,
                    Message = "Profile retrieved successfully.",
                    Data = dto,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<GeneralProfileReadDTO>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving profile: {ex.Message}",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<CustomerProfileDTO?>> GetCustomerRelatedInfoAsync(
            string userId
        )
        {
            var cst = await _customerRepo.GetFirstOrDefaultWithIncludesAsync(
                c => c.UserId == userId,
                c => c.Orders,
                c => c.PCAssembly,
                c => c.Deliveries,
                c => c.Maintenances,
                c => c.User
            );

            if (cst == null)
            {
                return new GeneralResponse<CustomerProfileDTO?>
                {
                    Success = false,
                    Message = "Customer not found",
                    Data = null,
                };
            }

            return new GeneralResponse<CustomerProfileDTO?>
            {
                Success = true,
                Message = "Customer profile retrieved successfully",
                Data = new CustomerProfileDTO
                {
                    UserId = cst.UserId,
                    FullName = cst.User?.FullName,
                    UserName = cst.User?.UserName,
                    Email = cst.User?.Email,
                    OrdersCount = cst.Orders?.Count ?? 0,
                    PCAssembliesCount = cst.PCAssembly?.Count ?? 0,
                    DeliveriesCount = cst.Deliveries?.Count ?? 0,
                    MaintenancesCount = cst.Maintenances?.Count ?? 0,
                },
            };
        }

        public async Task<GeneralResponse<TechCompanyProfileDTO?>> GetTechCompanyRelatedInfoAsync(
            string userId
        )
        {
            var comp = await _techCompanyRepo.GetFirstOrDefaultAsync(
                c => c.UserId == userId,
                q =>
                    q.Include(t => t.Products)
                        .ThenInclude(p => p.Category)
                        .Include(t => t.Products)
                        .ThenInclude(p => p.SubCategory)
                        .Include(t => t.Products)
                        .ThenInclude(p => p.Specifications)
                        .Include(t => t.Products)
                        .ThenInclude(p => p.Warranties)
                        .Include(t => t.Maintenances)
                        .Include(t => t.Deliveries)
                        .Include(t => t.PCAssemblies)
                        .Include(t => t.User)
            );

            if (comp == null)
            {
                return new GeneralResponse<TechCompanyProfileDTO?>
                {
                    Success = false,
                    Message = "Tech company not found",
                    Data = null,
                };
            }

            return new GeneralResponse<TechCompanyProfileDTO?>
            {
                Success = true,
                Message = "Tech company profile retrieved successfully",
                Data = new TechCompanyProfileDTO
                {
                    UserId = comp.UserId,
                    FullName = comp.User?.FullName,
                    UserName = comp.User?.UserName,
                    Rating = comp.Rating,
                    Description = comp.Description,
                    Products =
                        comp.Products?.Select(p => new ProductCardDTO
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Description = p.Description,
                                Price = p.Price,
                                DiscountPrice = p.DiscountPrice,
                                ImageUrl = p.ImageUrl ?? "assets/products/default-product.png",
                                Stock = p.Stock,
                                CreatedAt = p.CreatedAt,
                                UpdatedAt = p.UpdatedAt,
                                CategoryId = p.CategoryId,
                                CategoryName = p.Category?.Name,
                                SubCategoryId = p.SubCategoryId,
                                SubCategoryName = p.SubCategory?.Name,
                                TechCompanyId = p.TechCompanyId,
                                TechCompanyName = comp.User?.FullName,
                                TechCompanyAddress = comp.User?.Address ?? string.Empty,
                                TechCompanyUserId = comp.UserId,
                                TechCompanyImage = comp.User?.ProfilePhotoUrl ?? string.Empty,


                                Specifications = p.Specifications?.Select(s => new SpecificationDTO
                                    {
                                        Id = s.Id,
                                        Key = s.Key,
                                        Value = s.Value,
                                    })
                                    .ToList(),
                                Warranties = p.Warranties?.Select(w => new WarrantyDTO
                                    {
                                        Id = w.Id,
                                        Type = w.Type,
                                        Duration = w.Duration,
                                        Description = w.Description,
                                        StartDate = w.StartDate,
                                        EndDate = w.EndDate,
                                    })
                                    .ToList(),
                            })
                            .ToList() ?? new List<ProductCardDTO>(),
                    MaintenancesCount = comp.Maintenances?.Count ?? 0,
                    DeliveriesCount = comp.Deliveries?.Count ?? 0,
                    PCAssembliesCount = comp.PCAssemblies?.Count ?? 0,
                },
            };
        }

        public async Task<
            GeneralResponse<DeliveryPersonProfileDTO?>
        > GetDeliveryPersonRelatedInfoAsync(string userId)
        {
            var dp = await _deliveryPersonRepo.GetFirstOrDefaultWithIncludesAsync(
                d => d.UserId == userId,
                d => d.User,
                d => d.Deliveries,
                d => d.Offers
            );

            if (dp == null)
            {
                return new GeneralResponse<DeliveryPersonProfileDTO?>
                {
                    Success = false,
                    Message = "Delivery person not found",
                    Data = null,
                };
            }

            return new GeneralResponse<DeliveryPersonProfileDTO?>
            {
                Success = true,
                Message = "Delivery person profile retrieved successfully",
                Data = new DeliveryPersonProfileDTO
                {
                    UserId = dp.UserId,
                    UserName = dp.User?.UserName,
                    VehicleNumber = dp.VehicleNumber,
                    VehicleType = dp.VehicleType,
                    License = dp.License,
                    IsAvailable = dp.IsAvailable,
                    LastOnline = dp.LastOnline,
                    FullName = dp.User?.FullName,
                    VehicleImage = dp.VehicleImage,
                    DeliveriesCount = dp.Deliveries?.Count ?? 0,
                    OffersCount = dp.Offers?.Count ?? 0,
                },
            };
        }

        private GeneralProfileReadDTO MapToProfileDTO(AppUser u, IList<string> roles)
        {
            return new GeneralProfileReadDTO
            {
                UserId = u.Id,
                UserName = u.UserName ?? string.Empty,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                FullName = u.FullName,
                Address = u.Address,
                City = u.City,
                Country = u.Country,
                PostalCode = u.PostalCode,
                Latitude = u.Latitude,
                Longitude = u.Longitude,
                ProfilePhotoUrl = u.ProfilePhotoUrl,
                IsActive = u.IsActive,
                RoleNames = roles,
                // only if AppUser actually has these props
                CreatedAt = u.CreatedAt,
                LastLogin = u.LastLoginDate,
            };
        }
    }
}
