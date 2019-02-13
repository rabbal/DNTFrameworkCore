using System;

namespace DNTFrameworkCore.Domain.Entities.Tracking
{
    public interface IHasDeletionDateTime : ISoftDeleteEntity
    {
        DateTimeOffset? DeletionDateTime { get; set; }
    }
}