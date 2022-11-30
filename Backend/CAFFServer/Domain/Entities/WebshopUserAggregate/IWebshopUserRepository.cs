using CSONGE.Application.Pagination;
using System.Linq.Expressions;

namespace Domain.Entities.User
{
    public interface IWebshopUserRepository
    {
        Task<int> SaveChangesAsync();
        IQueryable<WebshopUser> GetAll();
        IQueryable<WebshopUser> GetAllIncluding(params Expression<Func<WebshopUser, object>>[] propertySelectors);
        Task<List<WebshopUser>> GetAllListAsync();
        Task<List<WebshopUser>> GetAllListAsync(Expression<Func<WebshopUser, bool>> predicate);
        Task<List<WebshopUser>> GetAllListOrderedAsync<TOrderKey>(Expression<Func<WebshopUser, bool>> predicate, Expression<Func<WebshopUser, TOrderKey>> orderKeySelector);
        Task<List<WebshopUser>> GetAllListOrderedDescendingAsync<TOrderKey>(Expression<Func<WebshopUser, bool>> predicate, Expression<Func<WebshopUser, TOrderKey>> orderKeySelector);
        Task<WebshopUser> InsertAsync(WebshopUser entity);
        Task<WebshopUser> UpdateAsync(WebshopUser entity);
        Task DeleteAsync(WebshopUser entity);
        Task<WebshopUser?> SingleOrDefaultAsync(Expression<Func<WebshopUser, bool>> predicate);
        Task<WebshopUser?> FirstOrDefaultAsync(Expression<Func<WebshopUser, bool>> predicate);
        WebshopUser SingleIncluding(Expression<Func<WebshopUser, bool>> predicate, params Expression<Func<WebshopUser, object>>[] propertySelectors);
        Task<WebshopUser> SingleIncludingAsync(Expression<Func<WebshopUser, bool>> predicate, params Expression<Func<WebshopUser, object>>[] propertySelectors);
        Task<WebshopUser> SingleAsync(Expression<Func<WebshopUser, bool>> predicate);
        Task<WebshopUser> FirstAsync(Expression<Func<WebshopUser, bool>> predicate);
        WebshopUser FirstIncluding(Expression<Func<WebshopUser, bool>> predicate, params Expression<Func<WebshopUser, object>>[] propertySelectors);
        Task<WebshopUser> FirstIncludingAsync(Expression<Func<WebshopUser, bool>> predicate, params Expression<Func<WebshopUser, object>>[] propertySelectors);
        WebshopUser? SingleOrDefaultIncluding(Expression<Func<WebshopUser, bool>> predicate, params Expression<Func<WebshopUser, object>>[] propertySelectors);
        Task<WebshopUser?> SingleOrDefaultIncludingAsync(Expression<Func<WebshopUser, bool>> predicate, params Expression<Func<WebshopUser, object>>[] propertySelectors);
        Task<WebshopUser?> FirstOrDefaultIncludingAsync(Expression<Func<WebshopUser, bool>> predicate, params Expression<Func<WebshopUser, object>>[] propertySelectors);
        Task<IPagedList<WebshopUser>> ToPagedListAsync(IQueryable<WebshopUser> query, int pageIndex, int pageSize);
    }
}
