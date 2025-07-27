using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace TechpertsSolutions.Core.DTOs.RegisterDTOs
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full name can only contain letters and spaces")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, and one number")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = null!;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Address must be between 10 and 200 characters")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string PhoneNumber { get; set; } = null!;

        //[Required(ErrorMessage = "Role is required")]
        //[EnumDataType(typeof(RoleType), ErrorMessage = "Invalid role selected")]
        //public RoleType Role { get; set; }
    }
}



