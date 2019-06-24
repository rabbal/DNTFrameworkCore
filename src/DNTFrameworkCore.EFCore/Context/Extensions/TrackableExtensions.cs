using DNTFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
namespace DNTFrameworkCore.EFCore.Context.Extensions
{
    /// <summary>
    ///     Extension methods for classes implementing ITrackable.
    /// </summary>
    public static class TrackableExtensions
    {
        /// <summary>
        ///     Convert TrackingState to EntityState.
        /// </summary>
        /// <param name="state">Trackable entity state</param>
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
                
                default:
                    return EntityState.Unchanged;
            }
        }

        /// <summary>
        ///     Convert EntityState to TrackingState.
        /// </summary>
        /// <param name="state">EF entity state</param>
        /// <returns>Trackable entity state</returns>
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
                
                default:
                    return TrackingState.Unchanged;
            }
        }
    }
}