using System;
using DNTFrameworkCore.Domain;

namespace ProjectName.Domain.Identity
{
    public class UserClaim : TrackableEntity, ICreationTracking, IModificationTracking
    {
        public const int MaxClaimTypeLength = 256;

        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }

        public User User { get; set; }
        public long UserId { get; set; }
    }
}