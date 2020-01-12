using System;
using DNTFrameworkCore.Cryptography;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.Configuration
{
    public class KeyValue : Entity, IModificationTracking, ICreationTracking, IHasRowIntegrity
    {
        public string Key { get; set; }
        [Encrypted] public string Value { get; set; }
        public string Hash { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
    }
}