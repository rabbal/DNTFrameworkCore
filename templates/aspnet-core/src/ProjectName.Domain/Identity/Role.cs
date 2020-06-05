using System;
using System.Collections.Generic;
using DNTFrameworkCore.Domain;

namespace ProjectName.Domain.Identity
{
    public class Role : Entity<long>, ICreationTracking, IModificationTracking, IHasRowVersion
    {
        public const int NameLength = 50;
        public const int DescriptionLength = 1024;

        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public byte[] Version { get; set; }

        public ICollection<UserRole> Users { get; set; } = new HashSet<UserRole>();
        public ICollection<RolePermission> Permissions { get; set; } = new HashSet<RolePermission>();
        public ICollection<RoleClaim> Claims { get; set; } = new HashSet<RoleClaim>();
    }
}