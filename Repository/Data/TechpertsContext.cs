using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Repository.Data
{
    public class TechpertsContext : DbContext
    {
        public TechpertsContext(DbContextOptions<TechpertsContext> options) : base(options) { }
    }
}
