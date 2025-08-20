using Core.DTOs;
using Core.DTOs.AdminDTOs;
using Core.DTOs.ProductDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class AdminUserManagementService : IAdminUserManagementService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public AdminUserManagementService(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<GeneralResponse<PaginatedDTO<UserListDTO>>> GetAllUsersAsync(
            int pageNumber,
            int pageSize,
            string? search,
            string? role,
            bool? isActive,
            string? sortBy,
            bool sortDesc
        )
        {
            try
            {
                var query = _userManager.Users.AsQueryable();

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(u =>
                        u.FullName.Contains(search)
                        || u.Email.Contains(search)
                        || u.PhoneNumber.Contains(search)
                        || u.Address.Contains(search)
                    );
                }

                // Apply role filter
                if (!string.IsNullOrWhiteSpace(role))
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                    var userIdsInRole = usersInRole.Select(u => u.Id).ToList();
                    query = query.Where(u => userIdsInRole.Contains(u.Id));
                }

                // Apply active status filter
                if (isActive.HasValue)
                {
                    query = query.Where(u => u.IsActive == isActive.Value);
                }

                // Apply sorting
                query = sortBy?.ToLower() switch
                {
                    "name" => sortDesc
                        ? query.OrderByDescending(u => u.FullName)
                        : query.OrderBy(u => u.FullName),
                    "email" => sortDesc
                        ? query.OrderByDescending(u => u.Email)
                        : query.OrderBy(u => u.Email),
                    _ => sortDesc ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id),
                };

                var totalCount = await query.CountAsync();
                var users = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var userDtos = new List<UserListDTO>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var userDto = new UserListDTO
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Address = user.Address,
                        City = user.City,
                        Country = user.Country,
                        ProfilePhotoUrl = user.ProfilePhotoUrl,
                        IsActive = user.IsActive,
                        Roles = roles.Any() ? roles.ToList() : new List<string> { "No Role" },
                        CreatedAt = user.CreatedAt,
                    };
                    userDtos.Add(userDto);
                }

                var paginatedResult = new PaginatedDTO<UserListDTO>
                {
                    Items = userDtos,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = totalCount,
                };

                return new GeneralResponse<PaginatedDTO<UserListDTO>>
                {
                    Success = true,
                    Message = "Users retrieved successfully.",
                    Data = paginatedResult,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaginatedDTO<UserListDTO>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving users.",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<UserListDTO>> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new GeneralResponse<UserListDTO>
                    {
                        Success = false,
                        Message = "User not found.",
                    };
                }

                var roles = await _userManager.GetRolesAsync(user);
                var userDto = new UserListDTO
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    City = user.City,
                    Country = user.Country,
                    ProfilePhotoUrl = user.ProfilePhotoUrl,
                    IsActive = user.IsActive,
                    Roles = roles.Any() ? roles.ToList() : new List<string> { "No Role" },
                    CreatedAt = user.CreatedAt,
                };

                return new GeneralResponse<UserListDTO>
                {
                    Success = true,
                    Message = "User retrieved successfully.",
                    Data = userDto,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<UserListDTO>
                {
                    Success = false,
                    Message = "An error occurred while retrieving user.",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<string>> DeactivateUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "User not found.",
                    };
                }

                user.IsActive = false;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "Failed to deactivate user.",
                        Data = string.Join(", ", result.Errors.Select(e => e.Description)),
                    };
                }

                return new GeneralResponse<string>
                {
                    Success = true,
                    Message = "User deactivated successfully.",
                    Data = userId,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while deactivating user.",
                    Data = ex.Message,
                };
            }
        }

        public async Task<GeneralResponse<string>> ActivateUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "User not found.",
                    };
                }

                user.IsActive = true;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return new GeneralResponse<string>
                    {
                        Success = false,
                        Message = "Failed to activate user.",
                        Data = string.Join(", ", result.Errors.Select(e => e.Description)),
                    };
                }

                return new GeneralResponse<string>
                {
                    Success = true,
                    Message = "User activated successfully.",
                    Data = userId,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while activating user.",
                    Data = ex.Message,
                };
            }
        }

        public async Task<GeneralResponse<List<string>>> ChangeUserRolesAsync(
            string userId,
            List<string> newRoles
        )
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new GeneralResponse<List<string>>
                    {
                        Success = false,
                        Message = "User not found.",
                    };
                }

                // Validate roles
                var invalidRoles = newRoles
                    .Where(role => !_roleManager.RoleExistsAsync(role).Result)
                    .ToList();
                if (invalidRoles.Any())
                {
                    return new GeneralResponse<List<string>>
                    {
                        Success = false,
                        Message = "Some roles do not exist.",
                        Data = invalidRoles,
                    };
                }

                // Get current roles
                var currentRoles = await _userManager.GetRolesAsync(user);

                // Remove current roles
                if (currentRoles.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                    {
                        return new GeneralResponse<List<string>>
                        {
                            Success = false,
                            Message = "Failed to remove current roles.",
                            Data = currentRoles.ToList(),
                        };
                    }
                }

                // Add new roles
                var addResult = await _userManager.AddToRolesAsync(user, newRoles);
                if (!addResult.Succeeded)
                {
                    return new GeneralResponse<List<string>>
                    {
                        Success = false,
                        Message = "Failed to assign new roles.",
                        Data = newRoles,
                    };
                }

                return new GeneralResponse<List<string>>
                {
                    Success = true,
                    Message = "User roles updated successfully.",
                    Data = newRoles,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<List<string>>
                {
                    Success = false,
                    Message = "An error occurred while updating roles.",
                    Data = new List<string> { ex.Message },
                };
            }
        }

        public async Task<GeneralResponse<object>> GetUserStatisticsAsync()
        {
            try
            {
                var totalUsers = await _userManager.Users.CountAsync();
                // Note: IsActive is not stored in database, so we'll assume all users are active
                var activeUsers = totalUsers;
                var inactiveUsers = 0;

                var roleStats = new Dictionary<string, int>();
                var roles = await _roleManager.Roles.ToListAsync();

                foreach (var role in roles)
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                    roleStats[role.Name] = usersInRole.Count;
                }

                var statistics = new
                {
                    TotalUsers = totalUsers,
                    ActiveUsers = activeUsers,
                    InactiveUsers = inactiveUsers,
                    RoleDistribution = roleStats,
                };

                return new GeneralResponse<object>
                {
                    Success = true,
                    Message = "User statistics retrieved successfully.",
                    Data = statistics,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving user statistics.",
                    Data = null,
                };
            }
        }
    }
}
