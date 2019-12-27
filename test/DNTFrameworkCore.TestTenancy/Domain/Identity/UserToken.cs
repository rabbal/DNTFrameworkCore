namespace DNTFrameworkCore.TestTenancy.Domain.Identity
{
    public class UserToken : Entity
    {
        public const int MaxTokenHashLength = 256;
        public string TokenHash { get; set; }
        public DateTimeOffset TokenExpirationDateTime { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }
    }
}