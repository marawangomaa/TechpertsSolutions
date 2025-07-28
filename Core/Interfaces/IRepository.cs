using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes);

        Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            string? includeProperties = null);

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindWithIncludesAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        void RemoveRange(IEnumerable<T> entities);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);

        Task SaveChangesAsync();

        Task<T?> GetByIdWithIncludesAsync(string id, params Expression<Func<T, object>>[] includes);
    }
}
