using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Repository.Data;

namespace Repository
{
    class TechpertsContextFactory : IDesignTimeDbContextFactory<TechpertsContext>
    {
        public TechpertsContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TechpertsContext>();
            optionsBuilder.UseSqlServer("Server=.;Database=TechpertsSolutionsV2;Trusted_Connection=True;TrustServerCertificate=True;");

            return new TechpertsContext(optionsBuilder.Options);
        }
    }
}
