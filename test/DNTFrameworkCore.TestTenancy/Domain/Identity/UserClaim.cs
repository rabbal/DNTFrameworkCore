namespace DNTFrameworkCore.TestTenancy.Domain.Identity
{
    public class UserClaim : TrackableEntity, IModificationTracking
    {
        public const int MaxClaimTypeLength = 256;

        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public User User { get; set; }
        public long UserId { get; set; }
    }
}