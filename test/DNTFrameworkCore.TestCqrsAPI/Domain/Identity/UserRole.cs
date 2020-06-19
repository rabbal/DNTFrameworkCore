using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Identity
{
    public class UserRole : TrackableEntity<long>, IRowIntegrity, ICreationTracking, IModificationTracking
    {
        public long UserId { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
        public long RoleId { get; set; }
    }
}