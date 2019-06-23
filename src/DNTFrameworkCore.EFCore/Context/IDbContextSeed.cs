using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.EFCore.Context
{
    public interface IDbContextSeed : IScopedDependency
    {
        void Seed();
    }
}