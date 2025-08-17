using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Entities
{
    public class DeliveryClusterDriverOffer: BaseEntity
    {
        public string DeliveryClusterId { get; set; }
        public DeliveryCluster DeliveryCluster { get; set; }

        public string DriverId { get; set; }
        public DeliveryPerson Driver { get; set; }

        public DeliveryOfferStatus Status { get; set; } = DeliveryOfferStatus.Pending;

        public DateTime OfferTime { get; set; } = DateTime.Now;
        public DateTime? ResponseTime { get; set; }
        public decimal OfferedPrice { get; set; }
        public string? Notes { get; set; }
    }
}
