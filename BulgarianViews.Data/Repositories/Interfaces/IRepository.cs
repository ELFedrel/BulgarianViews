using BulgarianViews.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Data.Repositories.Interfaces
{
    public interface IRepository<TType, TId>
    {
        void Add(TType entity);
        Task AddAsync(TType entity);
        void AddRange(IEnumerable<TType> entities);
        Task AddRangeAsync(IEnumerable<TType> entities);

        bool Delete(TId id);
        Task<bool> DeleteAsync(TId id);

        bool Update(TType entity);
        Task<bool> UpdateAsync(TType entity);

        TType GetById(TId id);
        Task<TType> GetByIdAsync(TId id);
        IEnumerable<TType> GetAll();
        Task<IEnumerable<TType>> GetAllAsync();
        IQueryable<TType> GetAllAttached();

        IEnumerable<TType> Find(Expression<Func<TType, bool>> predicate);
        Task<IEnumerable<TType>> FindAsync(Expression<Func<TType, bool>> predicate);

        bool Exists(TId id);
        Task<bool> ExistsAsync(TId id);
        Task<int> CountAsync();
        Task<TType> GetByIdIncludingAsync(TId id, params Expression<Func<TType, object>>[] includeProperties);
        void RemoveRange(IEnumerable<TType> entities);
        Task<bool> DeleteByCompositeKeyAsync(Guid userId, Guid locationId);
    }
}
