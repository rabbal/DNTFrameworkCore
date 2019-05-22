using System;
using System.Collections.Generic;
using System.Linq;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.EntityFramework.Context
{
    public interface IHookEngine : IScopedDependency
    {
        void ExecutePostActionHooks(IEnumerable<HookedEntityEntry> modifiedEntries);
        void ExecutePreActionHooks(IEnumerable<HookedEntityEntry> modifiedEntries);
    }

    public class HookEngine : IHookEngine
    {
        private readonly IServiceProvider _provider;

        public HookEngine(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public void ExecutePostActionHooks(IEnumerable<HookedEntityEntry> modifiedEntries)
        {
            foreach (var entityEntry in modifiedEntries)
            {
                var entry = entityEntry;

                var hooks = _provider.GetServices<IPostActionHook>()
                    .Where(x => (x.HookState & entry.PreSaveState) == entry.PreSaveState);
                foreach (var hook in hooks)
                {
                    var metadata = new HookEntityMetadata(entityEntry.PreSaveState);
                    hook.Hook(entityEntry.Entity, metadata);
                }
            }
        }

        public void ExecutePreActionHooks(IEnumerable<HookedEntityEntry> modifiedEntries)
        {
            foreach (var entityEntry in modifiedEntries)
            {
                var entry = entityEntry; //Prevents access to modified closure

                var hooks = _provider.GetServices<IPreActionHook>()
                    .Where(x => (x.HookState & entry.PreSaveState) == entry.PreSaveState);
                foreach (var hook in hooks)
                {
                    var metadata = new HookEntityMetadata(entityEntry.PreSaveState);

                    hook.Hook(entityEntry.Entity, metadata);

                    if (metadata.HasStateChanged)
                        entityEntry.PreSaveState = metadata.State;
                }
            }
        }
    }
}