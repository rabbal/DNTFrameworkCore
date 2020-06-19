using System;
using System.Collections.Generic;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Identity
{
    public class User : Entity<long>, IRowVersion, IRowIntegrity, ICreationTracking, IModificationTracking
    {
        public const int MaxUserNameLength = 256;
        public const int MaxDisplayNameLength = 50;
        public const int MaxPasswordHashLength = 256;
        public const int MaxSecurityStampLength = 256;

        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string DisplayName { get; set; }
        public string NormalizedDisplayName { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoggedInDateTime { get; set; }
        public byte[] Version { get; set; }
        public ICollection<UserRole> Roles { get; set; } = new HashSet<UserRole>();
        public ICollection<UserToken> Tokens { get; set; } = new HashSet<UserToken>();
        public ICollection<UserPermission> Permissions { get; set; } = new HashSet<UserPermission>();
        public ICollection<UserClaim> Claims { get; set; } = new HashSet<UserClaim>();
        public override string ToString() => UserName;
    }
}