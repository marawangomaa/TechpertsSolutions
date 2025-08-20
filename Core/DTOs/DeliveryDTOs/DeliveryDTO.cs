using Core.DTOs.DeliveryPersonDTOs;
using Core.DTOs.OrderDTOs;
using Core.Entities;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs.CustomerDTOs;
using TechpertsSolutions.Core.Entities;

namespace Core.DTOs.DeliveryDTOs
{
    public class DeliveryDTO
    {
        public string Id { get; set; }
        public DeliveryStatus Status { get; set; }
        public decimal DeliveryFee { get; set; }
        public List<DeliveryClusterDTO> Clusters { get; set; }
        //public DeliveryDTO(Delivery delivery)
        //{
        //    Id = delivery.Id;
        //    Status = delivery.Status;
        //    DeliveryFee = delivery.DeliveryFee;
        //    Clusters = delivery.Clusters?.Select(cluster => new DeliveryClusterDTO(cluster)).ToList();
        //}
    }
}
