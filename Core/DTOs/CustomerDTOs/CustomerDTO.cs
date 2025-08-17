namespace TechpertsSolutions.Core.DTOs.CustomerDTOs
{
    public class CustomerDTO
    {
        public string Id { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? CartId { get; set; }
        public string? WishListId { get; set; }
        public List<string>? PCAssemblyIds { get; set; }
        public List<string>? OrderIds { get; set; }
        public List<string>? MaintenanceIds { get; set; }
        public List<string>? DeliveryIds { get; set; }
    }
}
