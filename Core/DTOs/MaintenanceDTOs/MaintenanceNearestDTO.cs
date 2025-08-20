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
        public double? Distance { get; set; }
    }
} 
