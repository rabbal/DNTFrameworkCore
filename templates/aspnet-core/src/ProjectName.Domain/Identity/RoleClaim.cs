using System;
using DNTFrameworkCore.Domain;

namespace ProjectName.Domain.Identity
{
    public class RoleClaim : TrackableEntity<long>, ICreationTracking, IModificationTracking
    {
        public const int ClaimTypeLength = 256;

        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }

        public Role Role { get; set; }
        public long RoleId { get; set; }
    }
}