using System;
using DNTFrameworkCore.Domain;

namespace ProjectName.Domain.Identity
{
    public class UserRole : TrackableEntity, ICreationTracking, IModificationTracking
    {
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
        public long RoleId { get; set; }
    }
}