using System;
using System.Runtime.Serialization;

namespace DNTFrameworkCore.Exceptions
{
    [Serializable]
    public class EntityNotFoundException : FrameworkException
    {
        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(string message)
            : base(message)
        {
        }

        public EntityNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public EntityNotFoundException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }

        public EntityNotFoundException(Type entityType, object id, Exception innerException = null)
            : base($"There is no such an entity. Entity type: {entityType.FullName}, id: {id}", innerException)
        {
            EntityType = entityType;
            Id = id;
        }

        public Type EntityType { get; }
        public object Id { get; }
    }
}