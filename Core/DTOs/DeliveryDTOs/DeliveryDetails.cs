using Core.DTOs.DeliveryPersonDTOs;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DeliveryDTOs
{
    public class DeliveryDetailsDTO
    {
        public string Id { get; set; }
        public string TrackingNumber { get; set; }
        public string DeliveryAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerName { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public string Notes { get; set; }
        public decimal? DeliveryFee { get; set; }
        public DeliveryOrderDTO Order { get; set; }
        public DeliveryPersonDTO DeliveryPerson { get; set; }
        public List<DeliveryTechCompanyDTO> TechCompanies { get; set; }
        public List<DeliveryClusterDTO> Clusters { get; set; }
    }
}
