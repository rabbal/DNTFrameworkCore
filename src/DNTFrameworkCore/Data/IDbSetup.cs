using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Data
{
    public interface IDbSetup : ITransientDependency
    {
        void Seed();
    }
}