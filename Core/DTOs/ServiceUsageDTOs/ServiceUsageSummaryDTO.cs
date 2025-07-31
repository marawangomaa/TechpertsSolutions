namespace Core.DTOs.ServiceUsageDTOs
{
    public class ServiceUsageSummaryDTO
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int UsageCount { get; set; }
    }

    public class UserServiceUsageDTO
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<ServiceUsageSummaryDTO> Services { get; set; } = new List<ServiceUsageSummaryDTO>();
    }
} 
