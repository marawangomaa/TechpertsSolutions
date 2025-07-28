using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.SubCategoryDTOs
{
    public class UpdateSubCategoryDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CategoryId { get; set; }
    }
}
