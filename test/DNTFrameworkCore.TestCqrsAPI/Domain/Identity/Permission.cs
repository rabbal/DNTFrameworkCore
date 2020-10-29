using System;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Identity
{
    public abstract class Permission : TrackableEntity<long>, IHasRowIntegrity, ICreationTracking,
        IModificationTracking
    {
        public const int MaxNameLength = 128;
        public string Name { get; set; }
        public bool IsGranted { get; set; } = true;
        
        
    }
}