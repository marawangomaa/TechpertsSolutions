namespace TechpertsSolutions.Core.DTOs.Customer
{
    public class CustomerDTO
    {
        public int Id { get; set; }
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

        public int? CartId { get; set; }
        public int? WishListId { get; set; }

        public List<int>? PCAssemblyIds { get; set; }
        public List<int>? OrderIds { get; set; }
        public List<int>? MaintenanceIds { get; set; }
        public int? DeliveryId { get; set; }
    }
}
