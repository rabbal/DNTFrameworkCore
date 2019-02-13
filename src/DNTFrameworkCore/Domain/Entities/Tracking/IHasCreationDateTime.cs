using System;

namespace DNTFrameworkCore.Domain.Entities.Tracking
{
    public interface IHasCreationDateTime
    {
        DateTimeOffset CreationDateTime { get; set; }
    }
}