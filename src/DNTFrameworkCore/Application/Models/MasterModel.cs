using System;
using DNTFrameworkCore.Domain.Entities;

namespace DNTFrameworkCore.Application.Models
{
    public abstract class MasterModel : MasterModel<int>
    {
    }

    public abstract class MasterModel<TKey> : Model<TKey>, IHaveRowVersion where TKey : IEquatable<TKey>
    {
        public byte[] RowVersion { get; set; }
    }
}