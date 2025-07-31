namespace Core.DTOs.MaintenanceDTOs
{
    public class MaintenanceNearestDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string TechCompanyName { get; set; }
        public string TechCompanyAddress { get; set; }
        public string TechCompanyPhone { get; set; }
        public double? Distance { get; set; } // in kilometers
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
    }

    public class CustomerLocationDTO
    {
        public string CustomerId { get; set; }
        public string CustomerAddress { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
} 
