using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DeliveryDTOs
{
    public class DeliveryClusterCreateDTO
    {
        public string DeliveryId { get; set; }
        public string TechCompanyId { get; set; }
        public string TechCompanyName { get; set; }

        public double DistanceKm { get; set; }
        public decimal Price { get; set; }

        public string AssignedDriverId { get; set; }

        public double? DropoffLatitude { get; set; }
        public double? DropoffLongitude { get; set; }

        public double? PickupLatitude { get; set; }
        public double? PickupLongitude { get; set; }

        public bool PickupConfirmed { get; set; } = false;
        public DateTime? PickupConfirmedAt { get; set; }

        public int SequenceOrder { get; set; }
        public DeliveryClusterTracking Tracking { get; set; }
    }
}
