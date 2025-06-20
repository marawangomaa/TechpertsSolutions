﻿using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class Customer 
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public Cart? Cart { get; set; }
        public WishList? WishList { get; set; }
        public ICollection<PCAssembly>? PCAssembly { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Maintenance>? Maintenances { get; set; }
        public int DeliveryId { get; set; }
        public Delivery? Delivery { get; set; }
    }
    
}
