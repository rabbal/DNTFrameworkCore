using System;

namespace DNTFrameworkCore.Domain
{
    public interface IMergeable
    {
        Guid TrackerId { get; set; }
    }
}