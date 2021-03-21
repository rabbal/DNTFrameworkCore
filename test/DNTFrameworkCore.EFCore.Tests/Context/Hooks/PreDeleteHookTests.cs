using DNTFrameworkCore.EFCore.Context.Hooks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace DNTFrameworkCore.EFCore.Tests.Context.Hooks
{
    [TestFixture]
    public class PreDeleteHookTests
    {
        private class SoftDeletePreDeleteHook : PreDeleteHook<ISoftDeleteEntity>
        {
            protected override void Hook(ISoftDeleteEntity entity, HookEntityMetadata metadata)
            {
                metadata.State = EntityState.Modified;
                entity.IsDeleted = true;
            }
        }

        [Test]
        public void Should_PreDeleteHook_Reassign_To_Modified_State()
        {
            var hook = new SoftDeletePreDeleteHook();
            var metadata = new HookEntityMetadata(EntityState.Deleted);
            var entity = new TrackingDeletedEntity();
            hook.Hook(entity, metadata);

            Assert.AreEqual(true, metadata.HasStateChanged);
            Assert.AreEqual(EntityState.Modified, metadata.State);
        }
    }
}