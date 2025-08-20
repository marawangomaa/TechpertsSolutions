using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.ProfileDTOs
{
    public class GeneralProfileReadDTO
    {
        // IdentityUser core fields
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        // Extended AppUser fields
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public bool IsActive { get; set; }
        // Roles and permissions
        public IList<string> RoleNames { get; set; }

        // Optional extra info
        public DateTime CreatedAt { get; set; } // if you track user registration date
        public DateTime? LastLogin { get; set; } // if you track last login activity
    }
}
