using Core.DTOs.AdminDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class AdminMapper
    {
        public static AdminReadDTO AdminReadDTOMapper(Admin admin) 
        {
            return new AdminReadDTO 
            {
                Id = admin.Id,
                UserId = admin.UserId,
                RoleId = admin.RoleId,
                UserName = admin.User?.UserName,
                RoleName = admin.Role?.Name
            };
        }

        public static AdminReadDTO MapToAdminReadDTO(Admin admin) 
        {
            return new AdminReadDTO 
            {
                Id = admin.Id,
                UserId = admin.UserId,
                RoleId = admin.RoleId,
                UserName = admin.User?.UserName,
                RoleName = admin.Role?.Name,
                PostalCode = admin.User?.PostalCode,
                City = admin.User?.City,
                Country = admin.User?.Country,
                Address = admin.User?.Address,
                Latitude = admin.User?.Latitude,
                Longitude = admin.User?.Longitude
            };
        }
    }
}
