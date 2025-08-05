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
        public async Task<T> GetByIdAsync(string id)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null)
                throw new KeyNotFoundException($"{typeof(T).Name} with ID={id} not found.");
            return entity;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // String-based Includes
        public async Task<IEnumerable<T>> GetAllAsync(string? includeProperties = null)
        {
            var query = ApplyStringIncludes(_dbSet, includeProperties);
            return await query.ToListAsync();
        }

        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null)
        {
            var query = ApplyStringIncludes(_dbSet.Where(predicate), includeProperties);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, string? includeProperties)
        {
            var query = ApplyStringIncludes(_dbSet.Where(predicate), includeProperties);
            return await query.ToListAsync();
        }

        // Expression-based Includes
        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            var query = ApplyExpressionIncludes(_dbSet, includes);
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes)
        {
            var query = ApplyExpressionIncludes(_dbSet, includes);
            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdWithIncludesAsync(string id, params Expression<Func<T, object>>[] includes)
        {
            var query = ApplyExpressionIncludes(_dbSet, includes);
            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var query = ApplyExpressionIncludes(_dbSet.Where(predicate), includes);
            return await query.ToListAsync();
        }

        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var query = ApplyExpressionIncludes(_dbSet.Where(predicate), includes);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<T?> GetFirstOrDefaultWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var query = ApplyExpressionIncludes(_dbSet.Where(predicate), includes);
            return await query.FirstOrDefaultAsync();
        }

        // Func-based Includes (IQueryable -> IQueryable)
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

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> includeBuilder)
        {
            var query = includeBuilder != null ? includeBuilder(_dbSet.Where(predicate)) : _dbSet.Where(predicate);
            return await query.ToListAsync();
        }

        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> includeBuilder)
        {
            var query = includeBuilder != null ? includeBuilder(_dbSet.Where(predicate)) : _dbSet.Where(predicate);
            return await query.FirstOrDefaultAsync();
        }

        // Func-based Includes with ThenInclude Support
        public async Task<IEnumerable<T>> GetAllWithIncludesAsync(Func<IQueryable<T>, IIncludableQueryable<T, object>> includeBuilder)
        {
            IQueryable<T> query = includeBuilder != null ? includeBuilder(_dbSet) : _dbSet;
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> includeBuilder)
        {
            var query = includeBuilder != null ? includeBuilder(_dbSet.Where(predicate)) : _dbSet.Where(predicate);
            return await query.ToListAsync();
        }

        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> includeBuilder)
        {
            var query = includeBuilder != null ? includeBuilder(_dbSet.Where(predicate)) : _dbSet.Where(predicate);
            return await query.FirstOrDefaultAsync();
        }

        // Basic Find & Existence Check
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        // CRUD
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // Helpers for Includes
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
            {
                query = query.Include(include);
            }
            return query;
        }



        

        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

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
    }
}