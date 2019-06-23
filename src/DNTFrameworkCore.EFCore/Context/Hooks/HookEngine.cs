using System;
using System.Collections.Generic;
using System.Linq;
using DNTFrameworkCore.Dependency;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.EFCore.Context.Hooks
{
    public interface IHookEngine : IScopedDependency
    {
        void RunPostHooks(IEnumerable<EntityEntry> entries);
        void RunPreHooks(IEnumerable<EntityEntry> entries);
    }

    public class HookEngine : IHookEngine
    {
        private readonly IServiceProvider _provider;

        public HookEngine(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public void RunPreHooks(IEnumerable<EntityEntry> entries)
        {
            RunHooks<IPreActionHook>(entries);
        }

        public void RunPostHooks(IEnumerable<EntityEntry> entries)
        {
            RunHooks<IPostActionHook>(entries);
        }

        private void RunHooks<THook>(IEnumerable<EntityEntry> entries) where THook : IHook
        {
            foreach (var entry in entries)
            {
                var hooks = _provider.GetServices<THook>()
                    .Where(x => (x.HookState & entry.State) == entry.State);
                foreach (var hook in hooks)
                {
                    var metadata = new HookEntityMetadata(entry);
                    hook.Hook(entry.Entity, metadata);
                }
            }
        }
    }
}