using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class Warranty : BaseEntity
    {
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string ProductId { get; set; }
        public Product? Product { get; set; }

        public Maintenance? Maintenance { get; set; }
    }
}
