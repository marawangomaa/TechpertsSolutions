using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Maintenance
{
    public class MaintenanceWarrantyDTO
    {
        public string Id { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; } = null!;

        public string? ProductName { get; set; }
    }
}
