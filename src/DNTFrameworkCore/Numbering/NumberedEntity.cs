using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.Numbering
{
    public class NumberedEntity : Entity
    {
        public string EntityName { get; set; }
        public long NextValue { get; set; }
    }
}