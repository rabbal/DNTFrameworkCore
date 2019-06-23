using System;
using DNTFrameworkCore.Domain.Tracking;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace DNTFrameworkCore.EntityFramework.Tests.Context.Hooks
{
    [TestFixture]
    public class PreUpdateHookTests
    {
        private class TimestampPreUpdateHook : PreUpdateHook<IHasModificationDateTime>
        {
            protected override void Hook(IHasModificationDateTime entity, HookEntityMetadata metadata)
            {
                entity.LastModificationDateTime = DateTimeOffset.UtcNow;
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
            var entity = new TimestampedSoftDeletedEntity();

            ((IHook) hook).Hook(entity, null);
            Assert.AreEqual(DateTimeOffset.UtcNow.Date, entity.LastModificationDateTime.Value.Date);
        }
    }
}