using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Admin
{
    public class AdminCreateDTO
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
}
