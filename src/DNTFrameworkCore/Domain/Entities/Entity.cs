﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNTFrameworkCore.Domain.Entities
{
    public abstract class Entity : Entity<int>
    {
    }

    public abstract class Entity<TKey> : IEntity
        where TKey : IEquatable<TKey>
    {
        private int? _hashCode;
        public TKey Id { get; protected set; }

        [NotMapped] public TrackingState TrackingState { get; set; }

        public override int GetHashCode()
        {
            if (IsTransient()) return base.GetHashCode();

            if (!_hashCode.HasValue) _hashCode = Id.GetHashCode() ^ 31; // XOR for random distribution

            return _hashCode.Value;
        }

        public bool IsTransient()
        {
            if (EqualityComparer<TKey>.Default.Equals(Id, default)) return true;

            //Workaround for EF Core since it sets int/long to min value when attaching to dbContext
            if (typeof(TKey) == typeof(int)) return Convert.ToInt32(Id) <= 0;

            if (typeof(TKey) == typeof(long)) return Convert.ToInt64(Id) <= 0;

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity<TKey> other)) return false;

            if (ReferenceEquals(this, other)) return true;

            if (GetType() != other.GetType()) return false;

            if (this is ITenantEntity tenantEntity &&
                other is ITenantEntity otherTenantEntity &&
                tenantEntity.TenantId != otherTenantEntity.TenantId)
                return false;

            if (IsTransient() || other.IsTransient()) return false;

            return Id.Equals(other.Id);
        }

        public override string ToString()
        {
            return $"[{GetType().Name} : {Id}]";
        }

        public static bool operator ==(Entity<TKey> left, Entity<TKey> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
        {
            return !(left == right);
        }
    }
}