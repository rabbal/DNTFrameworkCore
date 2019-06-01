using DNTFrameworkCore.Domain.Entities;
using System;
using System.Collections.Generic;

namespace DNTFrameworkCore.Application.Models
{
    public abstract class Model : Model<int>
    {
    }

    public abstract class Model<TKey> : IHasTrackingState where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
        public TrackingState TrackingState { get; set; }
        public bool IsNew() => EqualityComparer<TKey>.Default.Equals(Id, default) && TrackingState == TrackingState.Added;
        public bool IsModified() => !IsNew() && TrackingState == TrackingState.Modified;
        public bool IsUnchanged() => !IsNew() && TrackingState == TrackingState.Unchanged;
        public bool IsDeleted() => !IsNew() && TrackingState == TrackingState.Deleted;
    }
}