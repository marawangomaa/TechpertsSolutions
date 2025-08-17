using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DeliveryDTOs
{
    public class DeliveryTrackingDTO
    {
        public string DeliveryId { get; set; }
        public DeliveryStatus Status { get; set; }
        public double? CurrentLat { get; set; }
        public double? CurrentLng { get; set; }
        public DateTime? EstimatedArrival { get; set; }
        public List<DeliveryClusterTrackingDTO> Clusters { get; set; }
    }
}
