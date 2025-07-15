using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data;

namespace Repository
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly TechpertsContext context;
        private readonly DbSet<T> dbSet;

        public Repository(TechpertsContext _context)
        {
            context = _context;
            dbSet = context.Set<T>();
        }

        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var prop in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(prop.Trim());
                }
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<T> GetByIdAsync(string id)
        {
            IQueryable<T> query = ApplyIncludes();

            var entity = await query.FirstOrDefaultAsync(e => EF.Property<string>(e, "Id") == id);
            if (entity == null)
                throw new KeyNotFoundException($"{typeof(T).Name} with ID={id} not found.");

            return entity;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            IQueryable<T> query = ApplyIncludes();
            return await query.ToListAsync();
        }

        public async Task AddAsync(T entity) => await dbSet.AddAsync(entity);
        public void Update(T entity) => dbSet.Update(entity);
        public void Remove(T entity) => dbSet.Remove(entity);
        public async Task SaveChanges() => await context.SaveChangesAsync();
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => await dbSet.AnyAsync(predicate);

        private IQueryable<T> ApplyIncludes()
        {
            if (typeof(T) == typeof(Customer))
                return (IQueryable<T>)context.Customers
                    .Include(x => x.User)
                    .Include(x => x.Role)
                    .Include(x => x.Cart).ThenInclude(c => c.CartItems)
                    .Include(x => x.WishList).ThenInclude(w => w.WishListItems)
                    .Include(x => x.PCAssembly)
                    .Include(x => x.Orders)
                    .Include(x => x.Maintenances)
                    .Include(x => x.Delivery);

            if (typeof(T) == typeof(Admin))
                return (IQueryable<T>)context.Admins
                    .Include(x => x.User)
                    .Include(x => x.Role);

            if (typeof(T) == typeof(SalesManager))
                return (IQueryable<T>)context.SalesManagers
                    .Include(x => x.User)
                    .Include(x => x.Role)
                    .Include(x => x.Orders);

            if (typeof(T) == typeof(TechManager))
                return (IQueryable<T>)context.TechManagers
                    .Include(x => x.User)
                    .Include(x => x.Role)
                    .Include(x => x.ManagedProducts);

            if (typeof(T) == typeof(TechCompany))
                return (IQueryable<T>)context.TechCompanies
                    .Include(x => x.User)
                    .Include(x => x.Role)
                    .Include(x => x.Maintenances)
                    .Include(x => x.Deliveries)
                    .Include(x => x.Products);

            if (typeof(T) == typeof(Product))
                return (IQueryable<T>)context.Products
                    .Include(x => x.Category)
                    .Include(x => x.SubCategory)
                    .Include(x => x.Specifications)
                    .Include(x => x.Warranties)
                    .Include(x => x.TechManager)
                    .Include(x => x.StockControlManager)
                    .Include(x => x.CartItems)
                    .Include(x => x.WishListItems)
                    .Include(x => x.PCAssemblyItems)
                    .Include(x => x.OrderItems);

            if (typeof(T) == typeof(Order))
                return (IQueryable<T>)context.Orders
                    .Include(x => x.Customer).ThenInclude(c => c.User)
                    .Include(x => x._Cart)
                    .Include(x => x.OrderItems)
                    .Include(x => x.OrderHistory)
                    .Include(x => x.ServiceUsage)
                    .Include(x => x.Delivery)
                    .Include(x => x.SalesManager);

            if (typeof(T) == typeof(OrderHistory))
                return (IQueryable<T>)context.OrderHistories
                    .Include(x => x.Orders);

            if (typeof(T) == typeof(ServiceUsage))
                return (IQueryable<T>)context.ServiceUsages
                    .Include(x => x.Orders)
                    .Include(x => x.PCAssemblies);

            if (typeof(T) == typeof(PCAssembly))
                return (IQueryable<T>)context.PCAssemblies
                    .Include(x => x.Customer)
                    .Include(x => x.ServiceUsage)
                    .Include(x => x.PCAssemblyItems);

            if (typeof(T) == typeof(PCAssemblyItem))
                return (IQueryable<T>)context.PCAssemblyItems
                    .Include(x => x.Product)
                    .Include(x => x.Cart)
                    .Include(x => x.PCAssembly);

            if (typeof(T) == typeof(Cart))
                return (IQueryable<T>)context.Carts
                    .Include(x => x.Customer).ThenInclude(c => c.User)
                    .Include(x => x.CartItems).ThenInclude(ci => ci.Product)
                    .Include(x => x.WishListItems)
                    .Include(x => x.PCAssemblyItems)
                    .Include(x => x.Order);

            if (typeof(T) == typeof(WishList))
                return (IQueryable<T>)context.WishLists
                    .Include(x => x.Customer).ThenInclude(c => c.User)
                    .Include(x => x.WishListItems);

            if (typeof(T) == typeof(WishListItem))
                return (IQueryable<T>)context.WishListItems
                    .Include(x => x.Product)
                    .Include(x => x.Cart)
                    .Include(x => x.WishList);

            if (typeof(T) == typeof(Maintenance))
                return (IQueryable<T>)context.Maintenances
                    .Include(x => x.Warranty)
                    .Include(x => x.TechCompany)
                    .Include(x => x.Customer)
                    .Include(x => x.serviceUsage);

            if (typeof(T) == typeof(Warranty))
                return (IQueryable<T>)context.Warranties
                    .Include(x => x.Product)
                    .Include(x => x.Maintenance);

            if (typeof(T) == typeof(Specification))
                return (IQueryable<T>)context.Specifications
                    .Include(x => x.Product);

            if (typeof(T) == typeof(CartItem))
                return (IQueryable<T>)context.CartItems
                    .Include(x => x.Product)
                    .Include(x => x.Cart);

            if (typeof(T) == typeof(OrderItem))
                return (IQueryable<T>)context.OrderItems
                    .Include(x => x.Product)
                    .Include(x => x.Order);

            if (typeof(T) == typeof(Category))
                return (IQueryable<T>)context.Categories
                    .Include(x => x.SubCategories)
                    .Include(x => x.Products);

            if (typeof(T) == typeof(SubCategory))
                return (IQueryable<T>)context.SubCategories
                    .Include(x => x.Category)
                    .Include(x => x.Products);

            if (typeof(T) == typeof(Delivery))
                return (IQueryable<T>)context.Deliveries
                    .Include(x => x.Orders)
                    .Include(x => x.Customers)
                    .Include(x => x.TechCompanies);

            return dbSet.AsQueryable();
        }


        public async Task<IEnumerable<T>> FindByNameAsync(Expression<Func<T, bool>> predicate)
        {
            return await context.Set<T>().Where(predicate).ToListAsync();
        }

        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes)
        {
            throw new NotImplementedException();
        }
    }
}