using CSONGE.Domain.Entities;

namespace Domain.Entities.CaffFileAggregate
{
    public class CaffFile : Entity<Guid>
    {
        public string OriginalFileName { get; set; }
    }
}
