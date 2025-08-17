using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DeliveryDTOs
{
    public class DeliveryClusterUpdateDTO
    {
        public string Id { get; set; }

        public DeliveryClusterStatus Status { get; set; }

        public string? AssignedDriverId { get; set; }
        public string? AssignedDriverName { get; set; }
        public DateTime? AssignmentTime { get; set; }

        public double? DropoffLatitude { get; set; }
        public double? DropoffLongitude { get; set; }

        public int SequenceOrder { get; set; }
    }
}
