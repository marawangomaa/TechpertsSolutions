using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.AdminDTOs
{
    public class UserRolesUpdateDTO
    {
        public List<RoleType> Roles { get; set; }
    }
}
