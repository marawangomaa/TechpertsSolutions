using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Category
{
    public class CategoryUpdateDTO
    {
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
