using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.RoleDTOs
{
    public class RoleDTO
    {
        [Required]
        [EmailAddress]
        public string userEmail { get; set; }
    }
}
