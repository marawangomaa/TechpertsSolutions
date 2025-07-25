using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.TechManagerDTOs
{
    public class TechManagerReadDTO
    {
        public string Id { get; set; }
        public string? Specialization { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string? UserName { get; set; }
        public string? RoleName { get; set; }
    }
}
