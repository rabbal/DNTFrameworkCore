using System;
using System.Data;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Persistence
{
    public interface IDbConnectionFactory : IDisposable, IScopedDependency
    {
        IDbConnection Create();
    }
}