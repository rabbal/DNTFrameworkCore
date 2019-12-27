using System;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestAPI.Domain.Identity
{
    public class UserToken : Entity, IHasRowIntegrity, ICreationTracking, IModificationTracking
    {
        public const int MaxTokenHashLength = 256;
        public string TokenHash { get; set; }
        public DateTimeOffset TokenExpirationDateTime { get; set; }
        
        public string Hash { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }
    }
}