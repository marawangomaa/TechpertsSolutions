using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DeliveryDTOs
{
    public class DeliveryReadDTO
    {
        public string Id { get; set; }
        public DeliveryStatus Status { get; set; }
        public decimal DeliveryFee { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<DeliveryClusterDTO> Clusters { get; set; } = new List<DeliveryClusterDTO>();
    }
}
