namespace DNTFrameworkCore.MultiTenancy
{
    public class MultiTenancyOptions
    {
        public bool Enabled { get; set; }
        public MultiTenancyDatabaseStrategy DatabaseStrategy { get; set; }
    }
}