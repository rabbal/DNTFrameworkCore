using System;
using System.Collections.Generic;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.Application.Models
{
    public abstract class Model : Model<int>
    {
    }

    public abstract class Model<TKey> : ITrackable where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }

        public bool IsNew() =>
            EqualityComparer<TKey>.Default.Equals(Id, default) || TrackingState == TrackingState.Added;

        public bool IsModified() => !IsNew() && TrackingState == TrackingState.Modified;
        public bool IsUnchanged() => !IsNew() && TrackingState == TrackingState.Unchanged;
        public bool IsDeleted() => !IsNew() && TrackingState == TrackingState.Deleted;
    }
}