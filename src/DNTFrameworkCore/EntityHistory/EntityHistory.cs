using System;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.EntityHistory
{
    //todo: under development
    public class EntityHistory : Entity<Guid>, ICreationTracking
    {
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public string JsonOriginalValue { get; set; }
        public string JsonNewValue { get; set; }
        public string Command { get; set; }
    }
}