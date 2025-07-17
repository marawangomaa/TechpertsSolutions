using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;

namespace Core.Interfaces.Services
{
    public interface IRoleService
    {
        Task<GeneralResponse<bool>> CheckRoleAsync(string roleName);
        Task<GeneralResponse<string>> AssignRoleAsync(string userEmail, RoleType roleName);
        Task<GeneralResponse<string>> UnassignRoleAsync(string userEmail, RoleType roleName);
        Task<GeneralResponse<List<string>>> GetAllRolesAsync();
    }
}
