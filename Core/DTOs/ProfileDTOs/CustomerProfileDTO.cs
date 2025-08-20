using Core.DTOs.CartDTOs;
using Core.DTOs.DeliveryDTOs;
using Core.DTOs.MaintenanceDTOs;
using Core.DTOs.OrderDTOs;
using Core.DTOs.PCAssemblyDTOs;
using Core.DTOs.WishListDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.ProfileDTOs
{
    public class CustomerProfileDTO
    {
        public string UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int OrdersCount { get; set; } = 0;
        public int DeliveriesCount { get; set; } = 0;
        public int PCAssembliesCount { get; set; } = 0;
        public int MaintenancesCount { get; set; } = 0;
    }
}
