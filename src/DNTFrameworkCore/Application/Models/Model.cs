using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DNTFrameworkCore.Application.Models
{
    public abstract class Model : Model<int>
    {
    }

    public abstract class Model<TKey> where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        //Todo:[JsonIgnore] public Guid TrackingId { get; set; }
        [JsonIgnore] public virtual bool IsNew => EqualityComparer<TKey>.Default.Equals(Id, default);
    }
}