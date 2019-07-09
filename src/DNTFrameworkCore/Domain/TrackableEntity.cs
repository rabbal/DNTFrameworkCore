using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNTFrameworkCore.Domain
{
    public abstract class TrackableEntity : TrackableEntity<int>
    {
    }

    public abstract class TrackableEntity<TKey> : Entity<TKey>, ITrackable where TKey : IEquatable<TKey>
    {
        [NotMapped] public TrackingState TrackingState { get; set; }
        [NotMapped] public ICollection<string> ModifiedProperties { get; set; }
    }
}