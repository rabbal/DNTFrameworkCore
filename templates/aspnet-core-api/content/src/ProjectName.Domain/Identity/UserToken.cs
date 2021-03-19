using System;
using DNTFrameworkCore.Domain;

namespace ProjectName.Domain.Identity
{
    public class UserToken : TrackableEntity<long>, ICreationTracking, IModificationTracking
    {
        public const int TokenHashLength = 256;
        public string TokenHash { get; set; }
        public DateTime TokenExpirationDateTime { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
    }
}