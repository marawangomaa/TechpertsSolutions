using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ServiceUsage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CallCount { get; set; }
    }
}
