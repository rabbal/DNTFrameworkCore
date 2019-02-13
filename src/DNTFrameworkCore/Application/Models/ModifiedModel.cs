namespace DNTFrameworkCore.Application.Models
{
    public class ModifiedModel<TValue>
    {
        public TValue NewValue { get; set; }
        public TValue OriginalValue { get; set; }
    }
}