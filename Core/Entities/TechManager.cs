using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class TechManager : BaseEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Specialization { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string RoleId { get; set; }
        public AppRole Role { get; set; }

        public ICollection<Product>? ManagedProducts { get; set; }
    }
}
