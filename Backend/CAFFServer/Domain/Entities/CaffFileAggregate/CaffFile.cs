using CSONGE.Domain.Entities;
using Domain.Entities.User;

namespace Domain.Entities.CaffFileAggregate
{
    public class CaffFile : Entity<Guid>
    {
        public string OriginalFileName { get; set; }

        public Guid UploaderId { get; set; }
        public WebshopUser Uploader { get; set; }
        public List<WebshopUser> Customers { get; set; }
    }
}
