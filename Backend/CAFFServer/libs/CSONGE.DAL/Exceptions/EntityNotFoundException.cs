using System;
using System.Linq.Expressions;

namespace CSONGE.Dal.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public object Id { get; set; }
        public Type EntityType { get; set; }

        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(Type entityType, object id)
            : this(entityType, id, null)
        {
        }

        public EntityNotFoundException(Type entityType, string message)
            : base(message)
        {
            EntityType = entityType;
        }

        public EntityNotFoundException(Type entityType, object id, Exception innerException)
            : base($"Could not find the requested entity. Type: {entityType.FullName}, id: {id}", innerException)
        {
            EntityType = entityType;
            Id = id;
        }

        public EntityNotFoundException(string message)
            : base(message)
        {
        }

        public EntityNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public static EntityNotFoundException FromPredicate<T>(Type type, Expression<Func<T, bool>> predicate)
        {
            return new EntityNotFoundException(type, $"Could not find the requested entity that matches the given conditions: {predicate.ToString()}");
        }
    }
}
