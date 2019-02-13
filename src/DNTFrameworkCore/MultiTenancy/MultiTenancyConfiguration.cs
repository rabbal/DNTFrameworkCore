namespace DNTFrameworkCore.MultiTenancy
{
    public class MultiTenancyConfiguration
    {
        public bool Enabled { get; set; }
        public MultiTenancyDatabaseStrategy DatabaseStrategy { get; set; }
    }
}