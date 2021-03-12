using System;
using System.Collections.Generic;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestAPI.Domain.Identity
{
    public class User : Entity<long>, IHasRowVersion, IHasRowIntegrity, ICreationTracking, IModificationTracking
    {
        public const int MaxUserNameLength = 256;
        public const int MaxDisplayNameLength = 50;
        public const int MaxPasswordHashLength = 256;
        public const int MaxPasswordLength = 128;
        public const int MaxSecurityTokenLength = 128;

        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string DisplayName { get; set; }
        public string NormalizedDisplayName { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoggedInDateTime { get; set; }
        public string SecurityToken { get; set; } = Guid.NewGuid().ToString("N");
        public byte[] Version { get; set; }

        public ICollection<UserRole> Roles { get; set; } = new HashSet<UserRole>();
        public ICollection<UserToken> Tokens { get; set; } = new HashSet<UserToken>();
        public ICollection<UserPermission> Permissions { get; set; } = new HashSet<UserPermission>();
        public ICollection<UserClaim> Claims { get; set; } = new HashSet<UserClaim>();
        public override string ToString() => UserName;
    }
}