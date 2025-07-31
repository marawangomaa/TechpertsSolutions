using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Core.DTOs.CustomerDTOs
{
    public class UserProfileUpdateDTO
    {
        [StringLength(100, MinimumLength = 2)]
        public string? FullName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [StringLength(500, MinimumLength = 10)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        public IFormFile? ProfilePhoto { get; set; }
    }
} 
