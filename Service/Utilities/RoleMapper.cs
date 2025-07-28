using TechpertsSolutions.Core.DTOs;
using Microsoft.AspNetCore.Identity;
using TechpertsSolutions.Core.Entities;
using System.Collections.Generic;

namespace Service.Utilities
{
    public static class RoleMapper
    {
        public static GeneralResponse<List<string>> MapToRoleListResponse(IEnumerable<AppRole> roles)
        {
            var roleNames = roles?.Select(r => r.Name).ToList() ?? new List<string>();

            if (roleNames.Any())
            {
                return new GeneralResponse<List<string>>
                {
                    Success = true,
                    Message = "Roles retrieved successfully.",
                    Data = roleNames
                };
            }

            return new GeneralResponse<List<string>>
            {
                Success = false,
                Message = "No roles found.",
                Data = new List<string>()
            };
        }

        public static GeneralResponse<string> MapToRoleAssignmentResponse(bool success, string message, string data)
        {
            return new GeneralResponse<string>
            {
                Success = success,
                Message = message,
                Data = data
            };
        }

        public static GeneralResponse<bool> MapToRoleCheckResponse(bool exists)
        {
            return new GeneralResponse<bool>
            {
                Success = true,
                Message = exists ? "Role exists." : "Role does not exist.",
                Data = exists
            };
        }
    }
} 
