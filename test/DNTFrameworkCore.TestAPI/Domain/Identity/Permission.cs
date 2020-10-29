using System;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestAPI.Domain.Identity
{
    public abstract class Permission : Entity, IHasRowIntegrity, ICreationTracking, IModificationTracking
    {
        public const int MaxNameLength = 128;
        public string Name { get; set; }
        public bool IsGranted { get; set; } = true;
        
    }
}