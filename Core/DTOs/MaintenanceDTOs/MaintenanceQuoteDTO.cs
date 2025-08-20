using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.MaintenanceDTOs
{
    public class MaintenanceQuoteDTO
    {
        public string MaintenanceId { get; set; }
        public string TechCompanyId { get; set; }
        
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        
        public DateTime EstimatedCompletionDate { get; set; }
        public string? Notes { get; set; }
    }
}
