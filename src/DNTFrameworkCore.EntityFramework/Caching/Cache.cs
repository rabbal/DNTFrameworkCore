using System;

namespace DNTFrameworkCore.EntityFramework.Caching
{
    public class Cache
    {
        public string Id { get; set; }
        public byte[] Value { get; set; }
        public DateTimeOffset ExpiresAtTime { get; set; }
        public long? SlidingExpirationInSeconds { get; set; }
        public DateTimeOffset? AbsoluteExpiration { get; set; }
    }
}