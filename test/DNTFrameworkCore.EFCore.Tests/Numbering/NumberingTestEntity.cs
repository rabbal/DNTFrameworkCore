using System;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.EFCore.Tests.Numbering
{
    public class NumberingTestEntity : Entity<int>, INumberedEntity, ICreationTracking, IModificationTracking
    {
        public string Number { get; set; }
        public string NumberBasedOnBranchId { get; set; }
        public string NumberBasedOnBranchIdDateTime { get; set; }
        public string NumberBasedOnBranchIdCreatedDateTime { get; set; }
        public long BranchId { get; set; }
        public DateTime DateTime { get; set; }
    }
}