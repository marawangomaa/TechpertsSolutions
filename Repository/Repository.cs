using Core.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data;

namespace Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly TechpertsContext context;
        private readonly DbSet<T> dbSet;

        public Repository(TechpertsContext _context)
        {
            context = _context;
            dbSet = context.Set<T>();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            if (typeof(T) == typeof(Customer))
            {
                var customer = await context.Customers
                    .Include(c => c.User)
                    .Include(c => c.Role)
                    .Include(c => c.Cart)
                    .Include(c => c.WishList)
                    .Include(c => c.PCAssembly)
                    .Include(c => c.Orders)
                    .Include(c => c.Maintenances)
                    .Include(c => c.Delivery)
                    .FirstOrDefaultAsync(c => c.Id == id);

                return (T)(object)customer!;
            }
            if (typeof(T) == typeof(Admin)) 
            {
                var admin = await context.Admins
                    .Include(a => a.User)
                    .Include(a => a.Role)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }

            return await dbSet.FindAsync(id) ?? throw new KeyNotFoundException($"Entity of type {typeof(T).Name} with id={id} not found.");
        }

        public async Task<IEnumerable<T>> GetAllAsync() 
        {
            if (typeof(T) == typeof(Customer))
            {
                return (IEnumerable<T>)await context.Customers
                    .Include(c => c.User)
                    .Include(c => c.Role)
                    .Include(c => c.Cart)
                    .Include(c => c.WishList)
                    .Include(c => c.PCAssembly)
                    .Include(c => c.Orders)
                    .Include(c => c.Maintenances)
                    .Include(c => c.Delivery)
                    .ToListAsync();
            }
            if (typeof(T) == typeof(Admin)) 
            {
                return (IEnumerable<T>)await context.Admins
                    .Include(a => a.User)
                    .Include(a => a.Role)
                    .ToListAsync();
            }

            return await context.Set<T>().ToListAsync();
        }

        public async Task AddAsync(T entity) => await dbSet.AddAsync(entity);

        public void Update(T entity) => dbSet.Update(entity);

        public void Remove(T entity) => dbSet.Remove(entity);

        public async Task SaveChanges()
        {
           await context.SaveChangesAsync();
        }
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.AnyAsync(predicate);
        }
    }
}
