using System;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestAPI.Domain.Identity
{
    public class UserRole : TrackableEntity, IHasRowIntegrity, ICreationTracking, IModificationTracking
    {
        public string Hash { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
        public long RoleId { get; set; }
    }
}