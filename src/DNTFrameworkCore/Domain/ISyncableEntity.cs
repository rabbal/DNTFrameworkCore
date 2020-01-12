using System;

namespace DNTFrameworkCore.Domain
{
    public interface ISyncableEntity
    {
        Guid UniqueId { get; set; }
        DateTime? SyncedDateTime { get; set; }
    }
}