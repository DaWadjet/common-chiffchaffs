using CSONGE.Application.Extensions;
using CSONGE.Application.Pagination;
using CSONGE.Dal.Exceptions;
using CSONGE.Dal.Repository;
using Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Repositories.WebshopUserAggregate
{
    public class WebshopUserRepository : IWebshopUserRepository
    {
        private readonly DbSet<WebshopUser> dbSet;
        private readonly WebshopDbContext context;

        public WebshopUserRepository(WebshopDbContext context)
        {
            dbSet = context.Set<WebshopUser>();
            this.context = context;
        }


        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }

        public IQueryable<WebshopUser> GetAll()
        {
            return GetAllIncluding();
        }

        public IQueryable<WebshopUser> GetAllIncluding(params Expression<Func<WebshopUser, object>>[] propertySelectors)
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

        public Task<List<WebshopUser>> GetAllListAsync()
        {
            return GetAll().ToListAsync();
        }

        public Task<List<WebshopUser>> GetAllListAsync(Expression<Func<WebshopUser, bool>> predicate)
        {
            return GetAll().Where(predicate).ToListAsync();
        }

        public Task<List<WebshopUser>> GetAllListOrderedAsync<TOrderKey>(Expression<Func<WebshopUser, bool>> predicate, Expression<Func<WebshopUser, TOrderKey>> orderKeySelector)
        {
            return GetAll()
                .Where(predicate)
                .OrderBy(orderKeySelector)
                .ToListAsync();
        }

        public Task<List<WebshopUser>> GetAllListOrderedDescendingAsync<TOrderKey>(Expression<Func<WebshopUser, bool>> predicate, Expression<Func<WebshopUser, TOrderKey>> orderKeySelector)
        {
            return GetAll()
                .Where(predicate)
                .OrderByDescending(orderKeySelector)
                .ToListAsync();
        }

        public WebshopUser Insert(WebshopUser entity)
        {
            return dbSet.Add(entity).Entity;
        }

        public async Task<WebshopUser> InsertAsync(WebshopUser entity)
        {
            var _entity = dbSet.Add(entity).Entity;
            await context.SaveChangesAsync();
            return _entity;
        }

        public WebshopUser Update(WebshopUser entity)
        {
            AttachIfNot(entity);
            context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public async Task<WebshopUser> UpdateAsync(WebshopUser entity)
        {
            AttachIfNot(entity);
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return entity;
        }

        public void Delete(WebshopUser entity)
        {
            AttachIfNot(entity);
            dbSet.Remove(entity);
        }
        public async Task DeleteAsync(WebshopUser entity)
        {
            AttachIfNot(entity);
            dbSet.Remove(entity);
            await context.SaveChangesAsync();
        }

        protected virtual void AttachIfNot(WebshopUser entity)
        {
            var entry = context.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry != null)
            {
                return;
            }

            dbSet.Attach(entity);
        }

        public Task<WebshopUser> SingleOrDefaultAsync(Expression<Func<WebshopUser, bool>> predicate)
        {
            return GetAll().SingleOrDefaultAsync(predicate);
        }

        public Task<WebshopUser> FirstOrDefaultAsync(Expression<Func<WebshopUser, bool>> predicate)
        {
            return GetAll().FirstOrDefaultAsync(predicate);
        }

        public WebshopUser SingleIncluding(Expression<Func<WebshopUser, bool>> predicate, params Expression<Func<WebshopUser, object>>[] propertySelectors)
        {
            var entity = SingleOrDefaultIncluding(predicate, propertySelectors);

            return entity ?? throw EntityNotFoundException.FromPredicate(typeof(WebshopUser), predicate);
        }

        public async Task<WebshopUser> SingleIncludingAsync(Expression<Func<WebshopUser, bool>> predicate, params Expression<Func<WebshopUser, object>>[] propertySelectors)
        {
            var entity = await SingleOrDefaultIncludingAsync(predicate, propertySelectors);

            return entity ?? throw EntityNotFoundException.FromPredicate(typeof(WebshopUser), predicate);
        }

        public async Task<WebshopUser> SingleAsync(Expression<Func<WebshopUser, bool>> predicate)
        {
            var entity = await SingleOrDefaultAsync(predicate);

            return entity ?? throw EntityNotFoundException.FromPredicate(typeof(WebshopUser), predicate);
        }

        public async Task<WebshopUser> FirstAsync(Expression<Func<WebshopUser, bool>> predicate)
        {
            var entity = await FirstOrDefaultAsync(predicate);

            return entity ?? throw EntityNotFoundException.FromPredicate(typeof(WebshopUser), predicate);
        }

        public WebshopUser FirstIncluding(Expression<Func<WebshopUser, bool>> predicate, params Expression<Func<WebshopUser, object>>[] propertySelectors)
        {
            var entity = FirstOrDefaultIncluding(predicate, propertySelectors);

            return entity ?? throw EntityNotFoundException.FromPredicate(typeof(WebshopUser), predicate);
        }

        public async Task<WebshopUser> FirstIncludingAsync(Expression<Func<WebshopUser, bool>> predicate, params Expression<Func<WebshopUser, object>>[] propertySelectors)
        {
            var entity = await FirstOrDefaultIncludingAsync(predicate, propertySelectors);

            return entity ?? throw EntityNotFoundException.FromPredicate(typeof(WebshopUser), predicate);
        }

        public WebshopUser? SingleOrDefaultIncluding(Expression<Func<WebshopUser, bool>> predicate, params Expression<Func<WebshopUser, object>>[] propertySelectors)
        {
            return GetAllIncluding(propertySelectors).SingleOrDefault(predicate);
        }

        public Task<WebshopUser?> SingleOrDefaultIncludingAsync(Expression<Func<WebshopUser, bool>> predicate, params Expression<Func<WebshopUser, object>>[] propertySelectors)
        {
            return GetAllIncluding(propertySelectors).SingleOrDefaultAsync(predicate);
        }

        public WebshopUser? FirstOrDefaultIncluding(Expression<Func<WebshopUser, bool>> predicate, params Expression<Func<WebshopUser, object>>[] propertySelectors)
        {
            return GetAllIncluding(propertySelectors).FirstOrDefault(predicate);
        }

        public Task<WebshopUser?> FirstOrDefaultIncludingAsync(Expression<Func<WebshopUser, bool>> predicate, params Expression<Func<WebshopUser, object>>[] propertySelectors)
        {
            return GetAllIncluding(propertySelectors).FirstOrDefaultAsync(predicate);
        }

        public Task<IPagedList<WebshopUser>> ToPagedListAsync(IQueryable<WebshopUser> query, int pageIndex, int pageSize)
        {
            return query.ToPagedListAsync(pageIndex, pageSize);
        }
    }
}