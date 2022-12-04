using CSONGE.Domain.Entities;
using Domain.Entities.CaffFileAggregate;
using Domain.Entities.CommentAggregate;
using Domain.Entities.User;

namespace Domain.Entities.ProductAggregate
{
    public class Product : Entity<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }

        public Guid UploaderId { get; set; }
        public WebshopUser Uploader { get; set; }

        public Guid? CaffFileId { get; set; }
        public CaffFile? CaffFile { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
