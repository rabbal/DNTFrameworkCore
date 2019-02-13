using DNTFrameworkCore.Domain.Entities.Tracking;

namespace DNTFrameworkCore.Domain.Entities.Extensions
{
    public static class EntityExtensions
    {
        public static bool IsNullOrDeleted(this ISoftDeleteEntity entity)
        {
            return entity == null || entity.IsDeleted;
        }

        public static void UnDelete(this ISoftDeleteEntity entity)
        {
            entity.IsDeleted = false;

            if (!(entity is IDeletionTracking deletionTracking)) return;

            deletionTracking.DeletionDateTime = null;
            deletionTracking.DeleterUserId = null;
            deletionTracking.DeleterBrowserName = null;
            deletionTracking.DeleterIp = null;
        }
    }
}