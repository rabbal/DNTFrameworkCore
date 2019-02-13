namespace DNTFrameworkCore.MultiTenancy
{
    public interface ITenant
    {
        TenantInfo Value { get; }
    }
}