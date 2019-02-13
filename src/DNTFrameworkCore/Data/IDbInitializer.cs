using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Data
{
    public interface IDbInitializer : IScopedDependency
    {
        void Initialize();
    }
}