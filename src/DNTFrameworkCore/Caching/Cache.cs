using System;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.Caching
{
    public class Cache : Entity<string>
    {
        public byte[] Value { get; set; }
        public DateTimeOffset ExpiresAtTime { get; set; }
        public long? SlidingExpirationInSeconds { get; set; }
        public DateTimeOffset? AbsoluteExpiration { get; set; }
    }
}