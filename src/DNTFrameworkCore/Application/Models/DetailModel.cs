using System;
using DNTFrameworkCore.Domain.Entities;
using Newtonsoft.Json;

namespace DNTFrameworkCore.Application.Models
{
    public abstract class DetailModel : DetailModel<int>
    {
    }

    public abstract class DetailModel<TKey> : Model<TKey>
        where TKey : IEquatable<TKey>
    {
        public TrackingState TrackingState { get; set; }
        [JsonIgnore] public override bool IsNew => base.IsNew && TrackingState == TrackingState.Added;
        [JsonIgnore] public bool IsModified => !base.IsNew && TrackingState == TrackingState.Modified;
        [JsonIgnore] public bool IsUnchanged => !base.IsNew && TrackingState == TrackingState.Unchanged;
        [JsonIgnore] public bool IsDeleted => !base.IsNew && TrackingState == TrackingState.Deleted;
    }
}