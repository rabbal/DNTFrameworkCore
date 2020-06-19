using System;
using System.Collections.Generic;

namespace DNTFrameworkCore.Application
{
    public abstract class MasterModel : MasterModel<int>
    {
    }

    public abstract class MasterModel<TKey> : ReadModel<TKey> where TKey : IEquatable<TKey>
    {
        public byte[] Version { get; set; }
        public virtual bool IsNew() => EqualityComparer<TKey>.Default.Equals(Id, default);
    }
}