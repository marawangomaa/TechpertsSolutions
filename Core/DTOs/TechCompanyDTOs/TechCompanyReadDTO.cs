using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.TechCompanyDTOs
{
    public class TechCompanyReadDTO
    {
        public string Id { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? PostalCode { get; set; }
        public string? Website { get; set; }
        public string? Description { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string? UserName { get; set; }
        public string? RoleName { get; set; }
    }
}
