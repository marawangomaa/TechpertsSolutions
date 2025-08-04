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

        // Includes (String-based)
        Task<IEnumerable<T>> GetAllAsync(string? includeProperties = null);
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null);
        Task<IEnumerable<T>> FindWithStringIncludesAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null);

        // Includes (Expression-based)
        Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes);
        Task<T?> GetByIdWithIncludesAsync(string id, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<T?> GetFirstOrDefaultWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes); // 🔧 Added
        Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> includes);

        // Includes (Func-based for flexibility)
        Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>> includeBuilder);
        Task<IEnumerable<T>> GetAllAsyncIncluded(Func<IQueryable<T>, IQueryable<T>> includeBuilder);

        // Existence Check
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        // CRUD
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task SaveChangesAsync();

        // Basic Find
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    }
}
