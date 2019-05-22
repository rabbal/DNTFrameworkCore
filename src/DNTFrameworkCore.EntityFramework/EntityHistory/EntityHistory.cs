using System;
using DNTFrameworkCore.Domain.Entities.Tracking;

namespace DNTFrameworkCore.EntityFramework.EntityHistory
{
    //todo: under development
    public class EntityHistory : CreationTrackingEntity<Guid>
    {
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public string JsonOriginalValue { get; set; }
        public string JsonNewValue { get; set; }
        public string Command { get; set; }
    }
}