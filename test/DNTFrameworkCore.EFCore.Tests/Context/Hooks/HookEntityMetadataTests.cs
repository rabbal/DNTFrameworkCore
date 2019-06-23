using DNTFrameworkCore.EntityFramework.Context.Hooks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace DNTFrameworkCore.EntityFramework.Tests.Context.Hooks
{
    [TestFixture]
    public class HookEntityMetadataTests
    {
        [Test]
        public void HookEntityMetadata_HasEntityState()
        {
            var result = new HookEntityMetadata(EntityState.Deleted) {State = EntityState.Modified};
            Assert.AreEqual(EntityState.Modified, result.State);
        }

        [Test]
        public void HookEntityMetadata_OnlyShowsEntityStateChangeAfterModification()
        {
            var result = new HookEntityMetadata(EntityState.Deleted);
            Assert.AreEqual(false, result.HasStateChanged);
            result.State = EntityState.Modified;
            Assert.AreEqual(true, result.HasStateChanged);
        }

        [Test]
        public void HookEntityMetadata_EntityStateChangedIsFalse_AfterReassigningSameValue()
        {
            var result = new HookEntityMetadata(EntityState.Modified);
            Assert.AreEqual(false, result.HasStateChanged);
            result.State = EntityState.Modified;
            Assert.AreEqual(false, result.HasStateChanged);
        }
    }
}