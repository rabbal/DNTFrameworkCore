using System;

namespace DNTFrameworkCore.Domain
{
    public interface ISyncableEntity
    {
        Guid RowId { get; set; }
        DateTime? SyncedDateTime { get; set; }
    }
}