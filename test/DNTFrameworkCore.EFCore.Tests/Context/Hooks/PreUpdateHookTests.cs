using System;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Context.Extensions;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.Timing;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace DNTFrameworkCore.EFCore.Tests.Context.Hooks
{
    [TestFixture]
    public class PreUpdateHookTests
    {
        private class TimestampPreUpdateHook : PreUpdateHook<IModificationTracking>
        {
            public override string Name => nameof(TimestampPreUpdateHook);

            protected override void Hook(IModificationTracking entity, HookEntityMetadata metadata, IDbContext dbContext)
            {
                dbContext.PropertyValue(entity, EFCoreShadow.ModifiedDateTime, SystemTime.Now);
            }
        }

        [Test]
        public void PreUpdateHook_HasModifiedHookState()
        {
            var hook = new TimestampPreUpdateHook();
            Assert.AreEqual(EntityState.Modified, hook.HookState);
        }

        [Test]
        public void PreUpdateHook_InterfaceHookCallsIntoGenericMethod()
        {
            var hook = new TimestampPreUpdateHook();
            var entity = new TrackingDeletedEntity();

            ((IHook) hook).Hook(entity, null);
            Assert.AreEqual(DateTimeOffset.UtcNow.Date, entity.LastModificationDateTime.Value.Date);
        }
    }
}