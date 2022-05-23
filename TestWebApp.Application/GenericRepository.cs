using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TestWebApp.Infrastructure;

namespace TestWebApp.Application
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        private TestWebAppDbContext _db;

        public GenericRepository(TestWebAppDbContext db)
        {
            _db = db;
        }

        public virtual IQueryable<TEntity> Get<TProperty>(
            Expression<Func<TEntity, bool>> filters = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Expression<Func<TEntity, TProperty>> includes = null,
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = _db.Set<TEntity>();

            if (filters != null)
            {
                query = query.Where(filters);
            }

            if (includes != null)
            {
                query = query.Include(includes);
            }

            if (orderBy != null)
            {
                query = orderBy(query);

            }

            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page ?? 0) * (pageSize ?? 10))
                        .Take(pageSize ?? 10);
            }

            return query;
        }

        public virtual async Task<TEntity> GetByIdAsync(object key)
        {
            return await _db.Set<TEntity>().FindAsync(key);
        }

        public virtual async Task CreateAsync(TEntity entity)
        {
            await _db.Set<TEntity>().AddAsync(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            _db.Set<TEntity>().Remove(entity);
            _db.Entry(entity).State = EntityState.Deleted;
        }

        public virtual async Task DeleteAsync(object id)
        {
            TEntity entity = await GetByIdAsync(id);
            if (entity != null)
            {
                Delete(entity);
            }
        }

        public virtual void Update(TEntity entity)
        {
            _db.Set<TEntity>().Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }

        public virtual async Task<(int Total, int Filtered)> CountAsync(
            Expression<Func<TEntity, bool>> filters = null)
        {
            var totalCount = await _db.Set<TEntity>().AsNoTracking().CountAsync();
            var filteredCount = totalCount;

            if (filters != null)
            {
                filteredCount = await _db.Set<TEntity>().AsNoTracking().Where(filters).CountAsync();
            }

            return (totalCount, filteredCount);
        }
    }
}
