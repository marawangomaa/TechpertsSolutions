using System.ComponentModel.DataAnnotations;

namespace TechpertsSolutions.Core.DTOs.LoginDTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; } = false;
    }
}
