namespace DNTFrameworkCore.Runtime
{
    /// <summary>
    /// Interface to get a user identifier.
    /// </summary>
    public interface IUserIdentifier
    {
        /// <summary>
        /// Tenant Id of the user.
        /// Can be null for host users in a multi tenant application.
        /// </summary>
        long? TenantId { get; }

        /// <summary>
        /// Id of the user.
        /// </summary>
        long UserId { get; }
    }
}