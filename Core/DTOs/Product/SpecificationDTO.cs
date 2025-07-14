using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Product
{
    public class SpecificationDTO
    {
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
