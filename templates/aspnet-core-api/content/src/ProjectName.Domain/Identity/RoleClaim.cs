namespace ProjectName.Domain.Identity
{
    public class RoleClaim : Claim
    {
        public Role Role { get; set; }
        public long RoleId { get; set; }
    }
}