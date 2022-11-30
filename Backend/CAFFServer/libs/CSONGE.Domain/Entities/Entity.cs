namespace CSONGE.Domain.Entities
{
    public class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
    {
        public Entity()
        {
        }

        public Entity(TPrimaryKey id)
        {
            this.Id = id;
        }
    }
}
