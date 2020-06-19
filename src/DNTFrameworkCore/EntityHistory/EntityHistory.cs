using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.EntityHistory
{
    //Under development
    public class EntityHistory : ICreationTracking, IModificationTracking
    {
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public string JsonOriginalValue { get; set; }
        public string JsonNewValue { get; set; }
    }
}