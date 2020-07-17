using System;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestAPI.Domain.Identity
{
    public class UserToken : Entity, IHasRowIntegrity, ICreationTracking, IModificationTracking
    {
        public const int MaxTokenHashLength = 256;
        public string TokenHash { get; set; }
        public DateTime TokenExpirationDateTime { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
    }
}