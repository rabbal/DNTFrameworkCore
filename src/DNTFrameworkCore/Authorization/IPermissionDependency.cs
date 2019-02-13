namespace DNTFrameworkCore.Authorization
{
    /// <summary>
    /// Defines interface to check a dependency.
    /// </summary>
    public interface IPermissionDependency
    {
        /// <summary>
        /// Checks if permission dependency is satisfied.
        /// </summary>
        bool IsSatisfied(PermissionDependencyContext context);
    }
}