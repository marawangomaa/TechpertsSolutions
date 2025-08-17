using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Interfaces
{
    public interface IRepository<T> where T : class, IEntity
    {
        // Basic Gets
        Task<T> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();

        // String-based Includes
        Task<IEnumerable<T>> GetAllAsync(string? includeProperties = null);
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, string? includeProperties);

        // Expression-based Includes
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes);
        Task<T?> GetByIdWithIncludesAsync(string id, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<T?> GetFirstOrDefaultWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        // Func-based Includes (IQueryable -> IQueryable)
        Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>> includeBuilder);
        Task<IEnumerable<T>> GetAllAsyncIncluded(Func<IQueryable<T>, IQueryable<T>> includeBuilder);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> includeBuilder);
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> includeBuilder);

        // Func-based Includes with ThenInclude Support (IIncludableQueryable)
        Task<IEnumerable<T>> GetAllWithIncludesAsync(Func<IQueryable<T>, IIncludableQueryable<T, object>> includeBuilder);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> includeBuilder);
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> includeBuilder);

        // Basic Find & Existence Check
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        // CRUD Operations
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task UpdateRangeAsync(IEnumerable<T> entities);
        void RemoveRange(IEnumerable<T> entities);
        Task SaveChangesAsync();
        Task<IEnumerable<T>> FindWithStringIncludesAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null);

        // Includes (Expression-based)
        Task<IEnumerable<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    }
}
