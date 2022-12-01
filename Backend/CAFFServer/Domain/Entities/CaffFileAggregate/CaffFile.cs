using CSONGE.Domain.Entities;
using Domain.Entities.ProductAggregate;
using Domain.Enum;

namespace Domain.Entities.CaffFileAggregate
{
    public class CaffFile : Entity<Guid>
    {
        public string OriginalFileName { get; set; }
        public FileExtension Extension { get; set; }
    }
}
