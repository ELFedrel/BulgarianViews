using BulgarianViews.Data.Models;
using BulgarianViews.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Data.Repositories
{
    public class Repository<TType, TId> : IRepository<TType, TId>
    where TType : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<TType> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TType>();
        }

        public void Add(TType entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public async Task AddAsync(TType entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void AddRange(IEnumerable<TType> entities)
        {
            _dbSet.AddRange(entities);
            _context.SaveChanges();
        }

        public async Task AddRangeAsync(IEnumerable<TType> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public bool Delete(TId id)
        {
            var entity = GetById(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> DeleteAsync(TId id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool Update(TType entity)
        {
            try
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(TType entity)
        {
            try
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public TType GetById(TId id)
        {
            return _dbSet.Find(id);
        }

        public async Task<TType> GetByIdAsync(TId id)
        {
            return await _dbSet.FindAsync(id);
        }

        public IEnumerable<TType> GetAll()
        {
            return _dbSet.ToList();
        }

        public async Task<IEnumerable<TType>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public IQueryable<TType> GetAllAttached()
        {
            return _dbSet.AsQueryable();
        }

        public IEnumerable<TType> Find(Expression<Func<TType, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }

        public async Task<IEnumerable<TType>> FindAsync(Expression<Func<TType, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public bool Exists(TId id)
        {
            return _dbSet.Find(id) != null;
        }

        public async Task<bool> ExistsAsync(TId id)
        {
            return await _dbSet.FindAsync(id) != null;
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<TType> GetByIdIncludingAsync(TId id, params Expression<Func<TType, object>>[] includeProperties)
        {
            IQueryable<TType> query = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.FirstOrDefaultAsync(entity => EF.Property<TId>(entity, "Id").Equals(id));
        }

        public void RemoveRange(IEnumerable<TType> entities)
        {
            _dbSet.RemoveRange(entities);
            _context.SaveChanges();
        }
    }
}
