using DNTFrameworkCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EntityFramework.Context.Extensions
{
    /// <summary>
    /// Extension methods for classes implementing <see cref="IHaveTrackingState"/>.
    /// </summary>
    public static class TrackingStateExtensions
    {
        /// <summary>
        /// Convert TrackingState to EntityState.
        /// </summary>
        /// <param name="state">IHaveTrackingState entity state</param>
        /// <returns>EF entity state</returns>
        public static EntityState ToEntityState(this TrackingState state)
        {
            switch (state)
            {
                case TrackingState.Added:
                    return EntityState.Added;

                case TrackingState.Modified:
                    return EntityState.Modified;

                case TrackingState.Deleted:
                    return EntityState.Deleted;

                case TrackingState.Unchanged:
                    return EntityState.Unchanged;

                default:
                    return EntityState.Unchanged;
            }
        }

        /// <summary>
        /// Convert EntityState to TrackingState.
        /// </summary>
        /// <param name="state">EF entity state</param>
        /// <returns>IHaveTrackingState entity state</returns>
        public static TrackingState ToTrackingState(this EntityState state)
        {
            switch (state)
            {
                case EntityState.Added:
                    return TrackingState.Added;

                case EntityState.Modified:
                    return TrackingState.Modified;

                case EntityState.Deleted:
                    return TrackingState.Deleted;

                case EntityState.Unchanged:
                    return TrackingState.Unchanged;

                default:
                    return TrackingState.Unchanged;
            }
        }
    }
}