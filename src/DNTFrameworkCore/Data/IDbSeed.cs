using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Data
{
    public interface IDbSeed : IScopedDependency
    {
        void Seed();
    }
}