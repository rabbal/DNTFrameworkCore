using System;
using System.Collections.Generic;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestAPI.Domain.Identity
{
    public class Role : Entity<long>, IHasRowVersion, IHasRowIntegrity, ICreationTracking, IModificationTracking
    {
        public const int MaxNameLength = 50;
        public const int MaxDescriptionLength = 1024;

        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string Description { get; set; }
        public byte[] Version { get; set; }
        public string Hash { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }

        public ICollection<UserRole> Users { get; set; } = new HashSet<UserRole>();
        public ICollection<RolePermission> Permissions { get; set; } = new HashSet<RolePermission>();
        public ICollection<RoleClaim> Claims { get; set; } = new HashSet<RoleClaim>();
    }
}