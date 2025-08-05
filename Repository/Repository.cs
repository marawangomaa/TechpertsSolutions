using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data;

namespace Repository
{
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly TechpertsContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(TechpertsContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // Basic Get by ID
        public async Task<T> GetByIdAsync(string id) =>
            await _dbSet.FirstOrDefaultAsync(e => e.Id == id)
            ?? throw new KeyNotFoundException($"{typeof(T).Name} with ID={id} not found.");

        // Basic Get All
        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        // Get All with string includes
        public async Task<IEnumerable<T>> GetAllAsync(string? includeProperties)
        {
            var query = ApplyStringIncludes(_dbSet, includeProperties);
            return await query.ToListAsync();
        }

        // Get All with Func include builder
        public async Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>> includeBuilder)
        {
            var query = includeBuilder != null ? includeBuilder(_dbSet) : _dbSet;
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsyncIncluded(Func<IQueryable<T>, IQueryable<T>> includeBuilder)
        {
            var query = includeBuilder != null ? includeBuilder(_dbSet) : _dbSet;
            return await query.ToListAsync();
        }

        // Get All with expression includes
        public async Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes)
        {
            var query = ApplyExpressionIncludes(_dbSet, includes);
            return await query.ToListAsync();
        }

        // Get by ID with includes
        public async Task<T?> GetByIdWithIncludesAsync(string id, params Expression<Func<T, object>>[] includes)
        {
            var query = ApplyExpressionIncludes(_dbSet, includes);
            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

        // GetFirstOrDefault with string includes
        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string? includeProperties)
        {
            var query = ApplyStringIncludes(_dbSet.Where(predicate), includeProperties);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> includes)
        {
            // Apply the includes to the queryable before executing it
            IQueryable<T> query = _dbSet;
            query = includes(query);

            return await query.FirstOrDefaultAsync(predicate);
        }

        // 🔧 Added: GetFirstOrDefault with expression includes
        public async Task<T?> GetFirstOrDefaultWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var query = ApplyExpressionIncludes(_dbSet.Where(predicate), includes);
            return await query.FirstOrDefaultAsync();
        }

        // Find with predicate
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.Where(predicate).ToListAsync();

        // Find with string includes
        public async Task<IEnumerable<T>> FindWithStringIncludesAsync(Expression<Func<T, bool>> predicate, string? includeProperties)
        {
            var query = ApplyStringIncludes(_dbSet.Where(predicate), includeProperties);
            return await query.ToListAsync();
        }

        // Find with expression includes
        public async Task<IEnumerable<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var query = ApplyExpressionIncludes(_dbSet.Where(predicate), includes);
            return await query.ToListAsync();
        }

        // Check existence
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.AnyAsync(predicate);

        // Add/Update/Delete
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public void Update(T entity) => _dbSet.Update(entity);
        public void Remove(T entity) => _dbSet.Remove(entity);
        public void RemoveRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        // Helpers
        private IQueryable<T> ApplyStringIncludes(IQueryable<T> query, string? includeProperties)
        {
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var prop in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    var trimmedProp = prop.Trim();
                    query = query.Include(trimmedProp);
                }
            }
            return query;
        }

        private IQueryable<T> ApplyExpressionIncludes(IQueryable<T> query, params Expression<Func<T, object>>[] includes)
        {
            foreach (var include in includes)
                query = query.Include(include);
            return query;
        }
    }
}