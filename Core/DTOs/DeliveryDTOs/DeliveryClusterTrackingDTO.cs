using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DeliveryDTOs
{
    public class DeliveryClusterTrackingDTO
    {
        public string ClusterId { get; set; }
        public string DeliveryId { get; set; }

        public string TechCompanyId { get; set; }
        public string TechCompanyName { get; set; }

        public double DistanceKm { get; set; }
        public decimal Price { get; set; }

        public string AssignedDriverId { get; set; }
        public string DriverName { get; set; }
        public DateTime? AssignmentTime { get; set; }

        public double? DropoffLatitude { get; set; }
        public double? DropoffLongitude { get; set; }

        public int SequenceOrder { get; set; }

        public double EstimatedDistance { get; set; }
        public decimal EstimatedPrice { get; set; }

        public DeliveryClusterStatus Status { get; set; }
        public string Location { get; set; }
        public DateTime LastUpdated { get; set; }

        public bool PickupConfirmed { get; set; }
        public DateTime? PickupConfirmedAt { get; set; }
    }
}