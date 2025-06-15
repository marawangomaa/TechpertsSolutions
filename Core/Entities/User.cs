using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public int RoleId { get; set; }
        public Role? Role { get; set; }

        public Customer? Customer { get; set; }
        public Admin? Admin { get; set; }
        public TechManager? TechManager { get; set; }
        public SalesManager? SalesManager { get; set; }
        public StockControlManager? StockControlManager { get; set; }
        public TechCompany? TechCompany { get; set; }
    }
}

