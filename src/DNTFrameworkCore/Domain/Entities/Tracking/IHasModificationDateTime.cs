using System;

namespace DNTFrameworkCore.Domain.Entities.Tracking
{
    public interface IHasModificationDateTime
    {
        DateTimeOffset? LastModificationDateTime { get; set; }
    }
}