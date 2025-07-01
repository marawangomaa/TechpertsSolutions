using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class Admin : IdentityRole
    {
  
        public int Id { get; set; }

        public string? Notes { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
    
   
}
