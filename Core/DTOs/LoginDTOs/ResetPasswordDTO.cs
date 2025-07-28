using System.ComponentModel.DataAnnotations;

namespace TechpertsSolutions.Core.DTOs.LoginDTOs
{
    public class ResetPasswordDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Reset token is required")]
        [StringLength(500, ErrorMessage = "Token cannot exceed 500 characters")]
        public string Token { get; set; } = null!;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", 
            ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, and one number")]
        public string NewPassword { get; set; } = null!;
    }
}


