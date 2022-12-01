using CSONGE.Dal.Repository;
using Domain.Entities.ProductAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Repositories.ProductAggregate
{
    public class ProductRepository : RepositoryBase<WebshopDbContext, Product, Guid>, IProductRepository
    {
        public ProductRepository(WebshopDbContext context) : base(context)
        {
        }
    }
}
