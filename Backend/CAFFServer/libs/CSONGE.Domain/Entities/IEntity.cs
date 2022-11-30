namespace CSONGE.Domain.Entities
{
    public class IEntity<TPrimaryKey>
    {
        public TPrimaryKey Id { get; set; }
    }
}
