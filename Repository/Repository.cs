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
        private readonly TechpertsContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(TechpertsContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> GetByIdAsync(string id) =>
            await _dbSet.FirstOrDefaultAsync(e => e.Id == id)
            ?? throw new KeyNotFoundException($"{typeof(T).Name} with ID={id} not found.");

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
                query = query.Include(include);

            return await query.ToListAsync();
        }
        public async Task<T?> GetByIdWithIncludesAsync(string id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var prop in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(prop.Trim());
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.Where(predicate).ToListAsync();

        public async Task<IEnumerable<T>> FindWithIncludesAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.Where(predicate);

            foreach (var include in includes)
                query = query.Include(include);

            return await query.ToListAsync();
        }
        public async Task<IEnumerable<T>> FindByNameAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.AnyAsync(predicate);

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Remove(T entity) => _dbSet.Remove(entity);
        public void RemoveRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
