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
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        public string ProductId { get; set; } = string.Empty;
        public Product? Product { get; set; }
    }
}
