using CSONGE.Domain.Entities;
using Domain.Entities.User;

namespace Domain.Entities.CommentAggregate
{
    public class Comment : Entity<Guid>
    {
        public string Content { get; set; }
        public Guid CommenterId { get; set; }
        public WebshopUser Commenter { get; set; }
    }
}
