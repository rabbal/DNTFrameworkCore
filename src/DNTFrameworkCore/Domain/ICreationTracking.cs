using System;

namespace DNTFrameworkCore.Domain
{
    public interface ICreationTracking
    {
        DateTime CreatedDateTime { get; set; }
    }
}