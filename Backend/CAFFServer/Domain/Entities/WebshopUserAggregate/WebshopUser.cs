using Domain.Entities.CaffFileAggregate;
using Domain.Entities.CommentAggregate;
using Domain.Entities.ProductAggregate;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.User
{
    public class WebshopUser : IdentityUser<Guid>
    {
        public bool IsAdmin { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
        public List<CaffFile> BoughtFiles { get; set; } = new List<CaffFile>();
        public List<CaffFile> OwnFiles { get; set; } = new List<CaffFile>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}