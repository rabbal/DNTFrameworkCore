using System;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestAPI.Domain.Identity
{
    public class UserToken : Entity
    {
        public const int MaxAccessTokenHashLength = 256;
        public const int MaxRefreshTokenIdHashLength = 256;
        public const int MaxRefreshTokenIdHashSourceLength = 256;

        public string TokenHash { get; set; }
        public DateTimeOffset TokenExpirationDateTime { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }
    }
}