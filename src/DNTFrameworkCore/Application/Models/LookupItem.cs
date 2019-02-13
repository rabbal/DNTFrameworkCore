namespace DNTFrameworkCore.Application.Models
{
    public class LookupItem : LookupItem<string>
    {
    }

    public class LookupItem<TValue>
    {
        public TValue Value { get; set; }
        public string Text { get; set; }
        public bool Selected { get; set; }
    }
}