using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class Specification : BaseEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Key { get; set; }
        public string Value { get; set; }

        public string ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
