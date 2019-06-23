using System;
using DNTFrameworkCore.Domain.Tracking;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace DNTFrameworkCore.EntityFramework.Tests.Context.Hooks
{
    [TestFixture]
    public class PreInsertHookTests
    {
        private class TimestampPreInsertHook : PreInsertHook<IHasCreationDateTime>
        {
            protected override void Hook(IHasCreationDateTime entity, HookEntityMetadata metadata)
            {
                entity.CreationDateTime = DateTimeOffset.UtcNow;
            }
        }

        [Test]
        public void Should_PreInsertHook_Has_Added_HookState()
        {
            var hook = new TimestampPreInsertHook();
            Assert.AreEqual(EntityState.Added, hook.HookState);
        }

        [Test]
        public void Should_PreInsertHook_InterfaceHook_Calls_Into_GenericMethod()
        {
            var hook = new TimestampPreInsertHook();
            var entity = new TimestampedSoftDeletedEntity();

            ((IHook) hook).Hook(entity, null);
            Assert.AreEqual(DateTimeOffset.UtcNow.Date, entity.CreationDateTime.Date);
        }
    }
}