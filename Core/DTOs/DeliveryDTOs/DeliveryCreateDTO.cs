using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DeliveryDTOs
{
    public class DeliveryCreateDTO
    {
        public string OrderId { get; set; }
        public string customerId { get; set; }
        public double CustomerLatitude { get; set; }
        public double CustomerLongitude { get; set; }
        public List<DeliveryClusterCreateDTO> Clusters { get; set; } = new List<DeliveryClusterCreateDTO>();
    }
}
