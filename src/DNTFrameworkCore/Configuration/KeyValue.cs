using System;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.Configuration
{
    public class KeyValue : Entity, IModificationTracking, ICreationTracking, IHasRowIntegrity
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Hash { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
    }
}