using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TechpertsSolutions.Core.DTOs.LoginDTOs
{
    public class DeleteAccountDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
