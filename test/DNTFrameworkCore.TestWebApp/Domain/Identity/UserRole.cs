using DNTFrameworkCore.Domain.Entities.Tracking;

namespace DNTFrameworkCore.TestWebApp.Domain.Identity
{
    public class UserRole : CreationTrackingEntity
    {
        public long UserId { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
        public long RoleId { get; set; }
    }
}