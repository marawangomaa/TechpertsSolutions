using Core.Entities;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Core.Entities
{
    public class Delivery : BaseEntity
    {
        public string? TrackingNumber { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
        public string? Notes { get; set; }
        public decimal DeliveryFee { get; set; } = 0m;
        public int RetryCount { get; set; } = 0;
        public string? PickupAddress { get; set; }
        public DateTime? PickupDate { get; set; }

        public double? PickupLatitude { get; set; }
        public double? PickupLongitude { get; set; }
        public double? DropoffLatitude { get; set; }
        public double? DropoffLongitude { get; set; }

        public string? DeliveryPersonId { get; set; }
        public string? CustomerId { get; set; }
        public string OrderId { get; set; }

        public string? ParentDeliveryId { get; set; }
        public int SequenceNumber { get; set; } = 0;
        public int RouteOrder { get; set; } = 0;

        public bool PickupConfirmed { get; set; }
        public DateTime? PickupConfirmedAt { get; set; }

        public bool IsFinalLeg { get; set; } = false;

        public Order? Order { get; set; }
        public Customer? Customer { get; set; }
        public Delivery? ParentDelivery { get; set; }
        public DeliveryPerson? DeliveryPerson { get; set; }
        public DeliveryClusterTracking Tracking { get; set; }
        public List<Delivery> SubDeliveries { get; set; } = new List<Delivery>();
        public List<DeliveryOffer> Offers { get; set; } = new List<DeliveryOffer>();
        public List<TechCompany> TechCompanies { get; set; } = new List<TechCompany>();
        public List<DeliveryCluster> Clusters { get; set; } = new List<DeliveryCluster>();
    }
}

