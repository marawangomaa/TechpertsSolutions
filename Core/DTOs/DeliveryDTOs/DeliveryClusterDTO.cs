using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DeliveryDTOs
{
    public class DeliveryClusterDTO
    {
        public string Id { get; set; }

        public string DeliveryId { get; set; }

        public string TechCompanyId { get; set; }
        public string TechCompanyName { get; set; }

        public double DistanceKm { get; set; }
        public decimal Price { get; set; }

        public DeliveryClusterStatus Status { get; set; }

        public string? AssignedDriverId { get; set; }
        public string? AssignedDriverName { get; set; }
        public DateTime? AssignmentTime { get; set; }
        
        public int RetryCount { get; set; }
        public DateTime? LastRetryTime { get; set; }

        public double? DropoffLatitude { get; set; }
        public double? DropoffLongitude { get; set; }

        public double? PickupLatitude { get; set; }
        public double? PickupLongitude { get; set; }

        public int SequenceOrder { get; set; }
        public int DriverOfferCount { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public double? EstimatedDistance { get; set; }
        public decimal? EstimatedPrice { get; set; }

        public DeliveryClusterTrackingDTO? Tracking { get; set; }
        public List<DeliveryOfferDTO>? Offers { get; set; } = new List<DeliveryOfferDTO>();
    }
}
