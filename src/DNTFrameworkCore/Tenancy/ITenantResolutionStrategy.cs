namespace DNTFrameworkCore.Tenancy
{
    public interface ITenantResolutionStrategy
    {
        string TenantId();
    }
}