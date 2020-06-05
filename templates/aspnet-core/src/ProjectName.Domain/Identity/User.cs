using System;
using System.Collections.Generic;
using DNTFrameworkCore.Domain;

namespace ProjectName.Domain.Identity
{
    public class User : Entity<long>, ICreationTracking, IModificationTracking, IHasRowVersion
    {
        public const int UserNameLength = 256;
        public const int DisplayNameLength = 50;
        public const int PasswordHashLength = 256;
        public const int PasswordLength = 128;
        public const int SerialNumberLength = 128;

        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string DisplayName { get; set; }
        public string NormalizedDisplayName { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public string SecurityStamp { get; set; }
        public DateTime? LastLoggedInDateTime { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public byte[] Version { get; set; }

        public ICollection<UserRole> Roles { get; set; } = new HashSet<UserRole>();
        public ICollection<UserToken> Tokens { get; set; } = new HashSet<UserToken>();
        public ICollection<UserPermission> Permissions { get; set; } = new HashSet<UserPermission>();
        public ICollection<UserClaim> Claims { get; set; } = new HashSet<UserClaim>();

        public override string ToString() => UserName;

        public static string NewSecurityStamp()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}