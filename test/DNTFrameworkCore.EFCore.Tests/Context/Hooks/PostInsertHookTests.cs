using System;
using DNTFrameworkCore.Domain.Tracking;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shouldly;

namespace DNTFrameworkCore.EntityFramework.Tests.Context.Hooks
{
    [TestFixture]
    public class PostInsertHookTests
    {
        private class TimestampPostInsertHook : PostInsertHook<IHasCreationDateTime>
        {
            protected override void Hook(IHasCreationDateTime entity, HookEntityMetadata metadata)
            {
                entity.CreationDateTime = DateTimeOffset.UtcNow;
            }
        }

        [Test]
        public void Should_PreInsertHook_Has_Added_HookState()
        {
            var hook = new TimestampPostInsertHook();
            Assert.AreEqual(EntityState.Added, hook.HookState);
        }

        [Test]
        public void Should_PostInsertHook_InterfaceHook_Calls_Into_GenericMethod()
        {
            var hook = new TimestampPostInsertHook();
            var entity = new TimestampedSoftDeletedEntity();

            ((IHook) hook).Hook(entity, null);
            Assert.AreEqual(DateTimeOffset.UtcNow.Date, entity.CreationDateTime.Date);
        }

        [Test]
        public void
            Should_Not_PostInsertHook_InterfaceHook_Calls_Into_GenericMethod_When_Type_Of_Entity_Is_Not_Same_As_HookEntityType()
        {
            var hook = new TimestampPostInsertHook();
            var entity = new SimpleEntity();

            ((IHook) hook).Hook(entity, null);
            entity.CreationDateTime.Date.ShouldNotBe(DateTimeOffset.UtcNow.Date);
        }
    }
}