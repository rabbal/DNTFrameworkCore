using System;
using DNTFrameworkCore.Domain.Tracking;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shouldly;

namespace DNTFrameworkCore.EntityFramework.Tests.Context.Hooks
{
    [TestFixture]
    public class PostDeleteHookTests
    {
        private class TimestampPostDeleteHook : PostDeleteHook<IHasModificationDateTime>
        {
            protected override void Hook(IHasModificationDateTime entity, HookEntityMetadata metadata)
            {
                entity.LastModificationDateTime = DateTimeOffset.UtcNow;
            }
        }

        [Test]
        public void Should_PostDeleteHook_Has_Deleted_HookState()
        {
            var hook = new TimestampPostDeleteHook();
            Assert.AreEqual(EntityState.Deleted, hook.HookState);
        }

        [Test]
        public void Should_PostDeleteHook_InterfaceHook_Calls_Into_GenericMethod()
        {
            var hook = new TimestampPostDeleteHook();
            var entity = new TimestampedSoftDeletedEntity();

            ((IHook) hook).Hook(entity, null);
            Assert.AreEqual(DateTimeOffset.UtcNow.Date, entity.LastModificationDateTime.Value.Date);
        }

        [Test]
        public void
            Should_Not_PostDeleteHook_InterfaceHook_Calls_Into_GenericMethod_When_Type_Of_Entity_Is_Not_Same_As_HookEntityType()
        {
            var hook = new TimestampPostDeleteHook();
            var entity = new SimpleEntity();

            ((IHook) hook).Hook(entity, null);
            entity.LastModificationDateTime.HasValue.ShouldBeFalse();
        }
    }
}