using System.Security.Claims;
using DNTFrameworkCore.Domain.Entities.Tracking;
using DNTFrameworkCore.Runtime;

namespace DNTFrameworkCore.TestAPI.Domain.Identity
{
    /// <summary>
    /// Base Class For TPH Inheritance Strategy
    /// Storage of System's Permissions
    /// </summary>
    public abstract class Permission : ModificationTrackingEntity
    {
        public const int MaxNameLength = 128;

        /// <summary>
        /// Unique Name of the Permission
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicate This Permission Is Granted With Role/User or Not
        /// </summary>
        public bool IsGranted { get; set; } = true;

        public Claim ToClaim()
        {
            return new Claim(DNTClaimTypes.Permission, Name);
        }

        public void InitializeFromClaim(Claim other)
        {
            Name = other?.Value;
        }
    }
}