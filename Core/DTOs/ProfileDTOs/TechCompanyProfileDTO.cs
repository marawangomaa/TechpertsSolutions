using Core.DTOs.DeliveryDTOs;
using Core.DTOs.MaintenanceDTOs;
using Core.DTOs.PCAssemblyDTOs;
using Core.DTOs.ProductDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.ProfileDTOs
{
    public class TechCompanyProfileDTO
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public decimal? Rating { get; set; }
        public string? Description { get; set; }
        public List<ProductCardDTO> Products { get; set; }
        public int MaintenancesCount { get; set; } = 0;
        public int DeliveriesCount { get; set; } = 0;
        public int PCAssembliesCount { get; set; } = 0;
    }
}
