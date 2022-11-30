using CSONGE.Application.Extensions;
using CSONGE.Application.Pagination;
using CSONGE.Dal.Exceptions;
using CSONGE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CSONGE.Dal.Repository
{
    public class RepositoryBase<TDbContext, TEntity, TPrimaryKey>
        where TDbContext : DbContext
        where TEntity : IEntity<TPrimaryKey>
    {
        protected readonly TDbContext context;

        private readonly DbSet<TEntity> dbSet;

        public RepositoryBase(TDbContext context)
        {
            this.context = context;
            dbSet = context.Set<TEntity>();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }

        public IQueryable<TEntity> GetAll()
        {
            return GetAllIncluding();
        }

        public IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var query = dbSet.AsQueryable();

            if (propertySelectors != null)
            {
                foreach (var propertySelector in propertySelectors)
                {
                    query = query.Include(propertySelector);
                }
            }

            return query;
        }

        public Task<List<TEntity>> GetAllListAsync()
        {
            return GetAll().ToListAsync();
        }

        public Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToListAsync();
        }

        public Task<List<TEntity>> GetAllListOrderedAsync<TOrderKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TOrderKey>> orderKeySelector)
        {
            return GetAll()
                .Where(predicate)
                .OrderBy(orderKeySelector)
                .ToListAsync();
        }

        public Task<List<TEntity>> GetAllListOrderedDescendingAsync<TOrderKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TOrderKey>> orderKeySelector)
        {
            return GetAll()
                .Where(predicate)
                .OrderByDescending(orderKeySelector)
                .ToListAsync();
        }

        public TEntity Insert(TEntity entity)
        {
            return dbSet.Add(entity).Entity;
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            var _entity = dbSet.Add(entity).Entity;
            await context.SaveChangesAsync();
            return _entity;
        }

        public TEntity Update(TEntity entity)
        {
            AttachIfNot(entity);
            context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            AttachIfNot(entity);
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return entity;
        }

        public void Delete(TEntity entity)
        {
            AttachIfNot(entity);
            dbSet.Remove(entity);
        }
        public async Task DeleteAsync(TEntity entity)
        {
            AttachIfNot(entity);
            dbSet.Remove(entity);
            await context.SaveChangesAsync();
        }

        protected virtual void AttachIfNot(TEntity entity)
        {
            var entry = context.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry != null)
            {
                return;
            }

            dbSet.Attach(entity);
        }

        public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().SingleOrDefaultAsync(predicate);
        }

        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefaultAsync(predicate);
        }

        public TEntity SingleIncluding(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var entity = SingleOrDefaultIncluding(predicate, propertySelectors);

            return entity ?? throw EntityNotFoundException.FromPredicate(typeof(TEntity), predicate);
        }

        public async Task<TEntity> SingleIncludingAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var entity = await SingleOrDefaultIncludingAsync(predicate, propertySelectors);

            return entity ?? throw EntityNotFoundException.FromPredicate(typeof(TEntity), predicate);
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = await SingleOrDefaultAsync(predicate);

            return entity ?? throw EntityNotFoundException.FromPredicate(typeof(TEntity), predicate);
        }

        public async Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = await FirstOrDefaultAsync(predicate);

            return entity ?? throw EntityNotFoundException.FromPredicate(typeof(TEntity), predicate);
        }

        public TEntity FirstIncluding(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var entity = FirstOrDefaultIncluding(predicate, propertySelectors);

            return entity ?? throw EntityNotFoundException.FromPredicate(typeof(TEntity), predicate);
        }

        public async Task<TEntity> FirstIncludingAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var entity = await FirstOrDefaultIncludingAsync(predicate, propertySelectors);

            return entity ?? throw EntityNotFoundException.FromPredicate(typeof(TEntity), predicate);
        }

        public TEntity SingleOrDefaultIncluding(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return GetAllIncluding(propertySelectors).SingleOrDefault(predicate);
        }

        public Task<TEntity> SingleOrDefaultIncludingAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return GetAllIncluding(propertySelectors).SingleOrDefaultAsync(predicate);
        }

        public TEntity FirstOrDefaultIncluding(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return GetAllIncluding(propertySelectors).FirstOrDefault(predicate);
        }

        public Task<TEntity> FirstOrDefaultIncludingAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return GetAllIncluding(propertySelectors).FirstOrDefaultAsync(predicate);
        }

        protected Task<IPagedList<TEntity>> ToPagedListAsync(IQueryable<TEntity> query, int pageIndex, int pageSize)
        {
            return query.ToPagedListAsync(pageIndex, pageSize);
        }
    }
}
