using System.Linq.Expressions;

namespace Domain.Entities.ProductAggregate
{
    public interface IProductRepository
    {
        IQueryable<Product> GetAll(); 
        Task<Product> SingleAsync(Expression<Func<Product, bool>> predicate);
        Task<Product> UpdateAsync(Product entity);
        Task DeleteAsync(Product entity);
        Task<Product> InsertAsync(Product entity);

    }
}
