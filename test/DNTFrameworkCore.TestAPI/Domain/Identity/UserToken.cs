using System;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestAPI.Domain.Identity
{
    public class UserToken : Entity, IRowIntegrity, ICreationTracking, IModificationTracking
    {
        public const int MaxTokenHashLength = 256;
        public string TokenHash { get; set; }
        public DateTime TokenExpirationDateTime { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
    }
}