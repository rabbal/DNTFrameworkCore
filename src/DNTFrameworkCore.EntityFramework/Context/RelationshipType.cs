namespace DNTFrameworkCore.EntityFramework.Context
{
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