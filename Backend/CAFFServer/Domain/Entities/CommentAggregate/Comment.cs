using CSONGE.Domain.Entities;
using Domain.Entities.ProductAggregate;
using Domain.Entities.User;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace Domain.Entities.CommentAggregate
{
    public class Comment : Entity<Guid>
    {
        public string Content { get; set; }
        public Guid CommenterId { get; set; }
        public WebshopUser Commenter { get; set; }
        public Guid? ProductId { get; set; }
        public Product? Product { get; set; }

        public void Update(string content) 
        {
            Content = content;
        }
    }
}
