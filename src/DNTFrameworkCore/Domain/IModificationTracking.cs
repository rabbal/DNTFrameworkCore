using System;

namespace DNTFrameworkCore.Domain
{
    public interface IModificationTracking
    {
        DateTime? ModifiedDateTime { get; set; }
    }
}