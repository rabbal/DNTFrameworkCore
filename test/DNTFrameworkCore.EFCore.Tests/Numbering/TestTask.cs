using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.EntityFramework.Tests.Numbering
{
    public class TestTask : Entity<int>, INumberedEntity, ITenantEntity
    {
        public string Number { get; set; }
        public long TenantId { get; set; }
    }
}