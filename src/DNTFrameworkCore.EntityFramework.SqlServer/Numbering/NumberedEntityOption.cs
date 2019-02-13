namespace DNTFrameworkCore.EntityFramework.SqlServer.Numbering
{
    public class NumberedEntityOption
    {
        public string Prefix { get; set; }
        public int Start { get; set; } = 1;
        public int IncrementBy { get; set; } = 1;
    }
}