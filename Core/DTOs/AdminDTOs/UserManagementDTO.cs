using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTOs.AdminDTOs
{
    public class UserListDTO
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserStatusUpdateDTO
    {
        [Required]
        public bool IsActive { get; set; }
    }
} 
