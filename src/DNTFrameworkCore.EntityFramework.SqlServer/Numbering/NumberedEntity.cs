using DNTFrameworkCore.Domain.Entities;

namespace DNTFrameworkCore.EntityFramework.SqlServer.Numbering
{
    public class NumberedEntity : Entity<int>, ITenantEntity
    {
        public string EntityName { get; set; }
        public long NextNumber { get; set; }

        public long TenantId { get; set; }
    }
}