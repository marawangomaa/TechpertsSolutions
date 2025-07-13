namespace TechpertsSolutions.Core.DTOs.Customer
{
    public class CustomerDTO
    {
        public string Id { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }

        // IdentityUser fields via AppUser
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }

        // IdentityRole fields via AppRole
        public string? RoleName { get; set; }
        public string? RoleNotes { get; set; }

        public string? CartId { get; set; }
        public string? WishListId { get; set; }

        public List<string>? PCAssemblyIds { get; set; }
        public List<string>? OrderIds { get; set; }
        public List<string>? MaintenanceIds { get; set; }
        public string? DeliveryId { get; set; }
    }
}
