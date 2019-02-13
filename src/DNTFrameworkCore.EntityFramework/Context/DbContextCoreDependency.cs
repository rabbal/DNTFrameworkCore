using System;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Runtime;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EntityFramework.Context
{
    public class DbContextCoreDependency<TDbContext> : DbContextCoreDependency where TDbContext : DbContext
    {
        public DbContextCoreDependency(
            IHookEngine hookEngine,
            IUserSession session,
            DbContextOptions<TDbContext> options) : base(hookEngine, session, options)
        {
        }
    }

    public class DbContextCoreDependency : IScopedDependency
    {
        public DbContextOptions DbContextOptions { get; }
        public IUserSession Session { get; }
        public IHookEngine HookEngine { get; }

        public DbContextCoreDependency(IHookEngine hookEngine, IUserSession session,
            DbContextOptions options)
        {
            HookEngine = hookEngine ?? throw new ArgumentNullException(nameof(hookEngine));
            Session = session ?? throw new ArgumentNullException(nameof(session));
            DbContextOptions = options ?? throw new ArgumentNullException(nameof(options));
        }
    }
}