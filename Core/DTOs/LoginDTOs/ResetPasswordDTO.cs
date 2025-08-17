using System.ComponentModel.DataAnnotations;

namespace TechpertsSolutions.Core.DTOs.LoginDTOs
{
    public class ResetPasswordDTO
    {
        [Required(ErrorMessage = "Reset token is required")]
        [StringLength(500, ErrorMessage = "Token cannot exceed 500 characters")]
        public string Token { get; set; } = null!; // JWT token from email link

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "Password must be between 6 and 100 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$",
            ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, and one number")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
