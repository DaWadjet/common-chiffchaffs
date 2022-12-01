using CSONGE.Dal.Repository;
using Domain.Entities.CommentAggregate;

namespace Dal.Repositories.CommentAggregate
{
    public class CommentRepository : RepositoryBase<WebshopDbContext, Comment, Guid>, ICommentRepository
    {
        public CommentRepository(WebshopDbContext context) : base(context)
        {
        }
    }
}
