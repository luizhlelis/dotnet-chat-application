using System;

namespace ChatApi.Domain.Entities
{
    public abstract class IdentifiableEntity
    {
        public Guid Id { get; private set; }

        public IdentifiableEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}
