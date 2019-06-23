using System;
using DNTFrameworkCore.Domain.Tracking;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shouldly;

namespace DNTFrameworkCore.EntityFramework.Tests.Context.Hooks
{
    [TestFixture]
    public class PostUpdateHookTests
    {
        private class TimestampPostUpdateHook : PostUpdateHook<IHasModificationDateTime>
        {
            protected override void Hook(IHasModificationDateTime entity, HookEntityMetadata metadata)
            {
                entity.LastModificationDateTime = DateTimeOffset.UtcNow;
            }
        }

        [Test]
        public void Should_PostUpdateHook_Has_Modified_HookState()
        {
            var hook = new TimestampPostUpdateHook();
            Assert.AreEqual(EntityState.Modified, hook.HookState);
        }

        [Test]
        public void Should_PostInsertHook_InterfaceHook_Calls_Into_GenericMethod()
        {
            var hook = new TimestampPostUpdateHook();
            var entity = new TimestampedSoftDeletedEntity();

            ((IHook) hook).Hook(entity, null);
            Assert.AreEqual(DateTimeOffset.UtcNow.Date, entity.LastModificationDateTime.Value.Date);
        }

        [Test]
        public void
            Should_Not_PostUpdateHook_InterfaceHook_Calls_Into_GenericMethod_When_Type_Of_Entity_Is_Not_Same_As_HookEntityType()
        {
            var hook = new TimestampPostUpdateHook();
            var entity = new SimpleEntity();

            ((IHook) hook).Hook(entity, null);
            entity.LastModificationDateTime.HasValue.ShouldBeFalse();
        }
    }
}