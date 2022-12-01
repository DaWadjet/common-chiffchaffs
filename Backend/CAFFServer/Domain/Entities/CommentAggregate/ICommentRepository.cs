using System.Linq.Expressions;

namespace Domain.Entities.CommentAggregate
{
    public interface ICommentRepository
    {
        IQueryable<Comment> GetAll();
        Task<Comment> SingleAsync(Expression<Func<Comment, bool>> predicate);
        Task<Comment> UpdateAsync(Comment entity);
        Task DeleteAsync(Comment entity);
        Task<Comment> InsertAsync(Comment entity);
    }
}
