using System;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.EntityHistory
{
    //TODO: under development
    public class EntityHistory : Entity<Guid>, ICreationTracking
    {
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public string JsonOriginalValue { get; set; }
        public string JsonNewValue { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}