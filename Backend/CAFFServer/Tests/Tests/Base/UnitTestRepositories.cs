using Dal;
using Dal.Repositories.CommentAggregate;
using Dal.Repositories.ProductAggregate;
using Dal.Repositories.WebshopUserAggregate;
using Domain.Entities.CommentAggregate;
using Domain.Entities.ProductAggregate;
using Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Base
{
    public class UnitTestRepositories
    {
        public IProductRepository ProductRepository { get; set; }
        public ICommentRepository CommentRepository { get; set; }
        public IWebshopUserRepository WebshopUserRepository { get; set; }
        public UnitTestRepositories(WebshopDbContext webshopDbContext)
        {
            ProductRepository = new ProductRepository(webshopDbContext);
            CommentRepository = new CommentRepository(webshopDbContext);
            WebshopUserRepository = new WebshopUserRepository(webshopDbContext);
        }
    }
}
