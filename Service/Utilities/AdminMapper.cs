using Core.DTOs.Admin;
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
    }
}
