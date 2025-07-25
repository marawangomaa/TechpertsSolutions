using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.AdminDTOs
{
    public class AdminReadDTO
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
    }
}
