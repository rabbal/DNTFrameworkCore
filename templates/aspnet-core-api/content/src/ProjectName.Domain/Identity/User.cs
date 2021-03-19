using System;
using System.Collections.Generic;
using DNTFrameworkCore.Domain;

namespace ProjectName.Domain.Identity
{
    public class User : Entity<long>, IHasRowVersion, IHasRowIntegrity, ICreationTracking, IModificationTracking
    {
        public const int UserNameLength = 256;
        public const int DisplayNameLength = 50;
        public const int PasswordHashLength = 256;
        public const int PasswordLength = 128;
        public const int SecurityTokenLength = 128;

        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string DisplayName { get; set; }
        public string NormalizedDisplayName { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public string SecurityToken { get; set; } = NewSecurityToken();
        public DateTime? LastLoggedInDateTime { get; set; }

        public ICollection<UserRole> Roles { get; set; } = new HashSet<UserRole>();
        public ICollection<UserToken> Tokens { get; set; } = new HashSet<UserToken>();
        public ICollection<UserPermission> Permissions { get; set; } = new HashSet<UserPermission>();
        public ICollection<UserClaim> Claims { get; set; } = new HashSet<UserClaim>();
        public byte[] Version { get; set; }

        public override string ToString()
        {
            return UserName;
        }

        public void ResetSecurityToken()
        {
            SecurityToken = NewSecurityToken();
        }

        public static string NewSecurityToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static string NormalizeUserName(string userName)
        {
            return userName.ToUpperInvariant();
        }

        public static string NormalizeDisplayName(string userName)
        {
            return userName.ToUpperInvariant();
        }
    }
}