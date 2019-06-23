using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DNTFrameworkCore.EFCore.Context.Extensions
{
    /// <summary>
    /// Extension methods for INavigation.
    /// </summary>
    public static class NavigationExtensions
    {
        /// <summary>
        /// Infer relationship type from an INavigation.
        /// </summary>
        /// <param name="nav">Navigation property which can be used to navigate a relationship.</param>
        /// <returns>Type of relationship between entities; null if INavigation is null.</returns>
        public static RelationshipType? GetRelationshipType(this INavigation nav)
        {
            if (nav == null) return null;
            if (nav.ForeignKey.IsUnique)
                return RelationshipType.OneToOne;
            return nav.IsDependentToPrincipal() ? RelationshipType.OneToMany : RelationshipType.ManyToOne;
        }
    }
    
    /// <summary>
    /// Type of relationship between entities.
    /// </summary>
    public enum RelationshipType
    {
        /// <summary>Many to one relationship.</summary>
        ManyToOne,

        /// <summary>One to one relationship.</summary>
        OneToOne,

        /// <summary>Many to many relationship.</summary>
        ManyToMany,

        /// <summary>One to many relationship.</summary>
        OneToMany
    }
}