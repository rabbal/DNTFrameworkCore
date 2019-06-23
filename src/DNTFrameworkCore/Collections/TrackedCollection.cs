using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.Collections
{
    public static class TrackedCollectionExtensions
    {
        public static TrackedCollection<T> AsTrackedCollection<T>(this IEnumerable<T> items)
            where T : ITrackable
        {
            return new TrackedCollection<T>(items);
        }
    }

    public class TrackedCollection<T> : Collection<T> where T : ITrackable
    {
        private readonly IEnumerable<T> _items;

        public TrackedCollection(IEnumerable<T> items)
        {
            _items = items;
        }

        public IReadOnlyList<T> RemovedItems =>
            _items.Where(item => item.TrackingState == TrackingState.Deleted).ToList().AsReadOnly();

        public IReadOnlyList<T> NewItems =>
            _items.Where(item => item.TrackingState == TrackingState.Added).ToList().AsReadOnly();

        public IReadOnlyList<T> ModifiedItems =>
            _items.Where(item => item.TrackingState == TrackingState.Modified).ToList().AsReadOnly();

        public IReadOnlyList<T> UnchangedItems =>
            _items.Where(item => item.TrackingState == TrackingState.Unchanged).ToList().AsReadOnly();
    }
}