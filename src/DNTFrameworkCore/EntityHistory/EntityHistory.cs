using System;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.EntityHistory
{
    //Under development
    public class EntityHistory : IHasRowIntegrity, ICreationTracking
    {
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public string JsonOriginalValue { get; set; }
        public string JsonNewValue { get; set; }
        
        public DateTime CreatedDateTime { get; set; }
    }
}