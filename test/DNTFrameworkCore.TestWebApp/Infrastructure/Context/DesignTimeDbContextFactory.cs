using System;
using System.Collections.Generic;
using System.IO;
using DNTFrameworkCore.EntityFramework.Context;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using DNTFrameworkCore.MultiTenancy;
using DNTFrameworkCore.Runtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.TestWebApp.Infrastructure.Context
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ProjectDbContext>
    {
        public ProjectDbContext CreateDbContext(string[] args)
        {
            var services = new ServiceCollection();

            services.AddOptions();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddScoped<IUserSession, StubUserSession>();
            services.AddScoped<IHookEngine, StubHookEngine>();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var provider = services.BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<ProjectDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlServer(connectionString);

            return new ProjectDbContext(provider.GetService<IHookEngine>(),
                provider.GetService<IUserSession>(), builder.Options);
        }

        private class StubHookEngine : IHookEngine
        {
            public void ExecutePostActionHooks(IEnumerable<HookedEntityEntry> modifiedEntries)
            {
            }

            public void ExecutePreActionHooks(IEnumerable<HookedEntityEntry> modifiedEntries)
            {
            }
        }

        private class StubUserSession : IUserSession
        {
            public bool IsAuthenticated => throw new NotImplementedException();

            public long? UserId => throw new NotImplementedException();

            public string UserName => throw new NotImplementedException();

            public string UserDisplayName => throw new NotImplementedException();

            public string UserBrowserName => throw new NotImplementedException();

            public string UserIP => throw new NotImplementedException();

            public long? TenantId => null;

            public MultiTenancySides MultiTenancySide => throw new NotImplementedException();

            public long? ImpersonatorUserId => throw new NotImplementedException();

            public long? ImpersonatorTenantId => throw new NotImplementedException();

            public IReadOnlyList<string> Permissions => throw new NotImplementedException();

            public IReadOnlyList<string> Roles => throw new NotImplementedException();

            public long? BranchId => null;

            public IDisposable Use(long? tenantId, long? userId)
            {
                throw new NotImplementedException();
            }

            public bool IsInRole(string role)
            {
                throw new NotImplementedException();
            }

            public bool IsGranted(string permission)
            {
                throw new NotImplementedException();
            }
        }
    }
}