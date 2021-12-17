using System;

namespace ChatApi.Domain.Entities
{
    public abstract class IdentifiableEntity
    {
        public Guid Id { get; protected set; }

        public IdentifiableEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}
