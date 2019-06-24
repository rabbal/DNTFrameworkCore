using System;
using System.Collections.Generic;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestWebApp.Domain.Identity
{
    public class User : TrackableEntity<long>, IHasRowVersion, ICreationTracking, IModificationTracking
    {
        public const int MaxUserNameLength = 256;
        public const int MaxDisplayNameLength = 50;
        public const int MaxPasswordHashLength = 256;
        public const int MaxPasswordLength = 128;
        public const int MaxSerialNumberLength = 128;

        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string DisplayName { get; set; }
        public string NormalizedDisplayName { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset? LastLoggedInDateTime { get; set; }
        public byte[] RowVersion { get; set; }

        /// <summary>
        /// A random value that must change whenever a users credentials change (password,roles or permissions)
        /// </summary>
        public string SerialNumber { get; set; }

        public ICollection<UserRole> Roles { get; set; } = new HashSet<UserRole>();
        public ICollection<UserPermission> Permissions { get; set; } = new HashSet<UserPermission>();
        public ICollection<UserClaim> Claims { get; set; } = new HashSet<UserClaim>();

        public override string ToString() => UserName;

        public string NewSerialNumber()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}