using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Entities
{
    public class DeliveryCluster : BaseEntity
    {
        public string DeliveryId { get; set; }
        public Delivery Delivery { get; set; }

        public string TechCompanyId { get; set; }
        public string TechCompanyName { get; set; }
        public TechCompany TechCompany { get; set; }

        public double DistanceKm { get; set; }
        public decimal Price { get; set; }

        public DeliveryClusterStatus Status { get; set; } = DeliveryClusterStatus.Pending;

        public string? AssignedDriverId { get; set; }
        public string? AssignedDriverName { get; set; }
        public DeliveryPerson? AssignedDriver { get; set; }
        public DateTime? AssignmentTime { get; set; }

        public double? DropoffLatitude { get; set; }
        public double? DropoffLongitude { get; set; }

        public int SequenceOrder { get; set; } = 0;

        public double EstimatedDistance { get; set; }
        public decimal EstimatedPrice { get; set; }

        public bool PickupConfirmed { get; set; } = false;
        public DateTime? PickupConfirmedAt { get; set; }

        public DeliveryClusterTracking Tracking { get; set; }

        public List<DeliveryClusterDriverOffer> DriverOffers { get; set; } = new List<DeliveryClusterDriverOffer>();
    }
}
