using System;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestAPI.Domain.Identity
{
    public class RoleClaim : TrackableEntity, IHasRowIntegrity, ICreationTracking, IModificationTracking
    {
        public const int MaxClaimTypeLength = 256;

        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public string Hash { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }

        public Role Role { get; set; }
        public long RoleId { get; set; }
    }
}