// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using System.Security.Claims;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Runtime
{
    /// <summary>
    ///     Defines some session information that can be useful for applications.
    /// </summary>
    public interface IUserSession : IScopedDependency
    {
        bool IsAuthenticated { get; }

        /// <summary>
        ///     Gets current UserId or null.
        ///     It can be null if no user logged in.
        /// </summary>
        string UserId { get; }

        /// <summary>
        ///     Gets current UserName or null.
        ///     It can be null if no user logged in.
        /// </summary>
        string UserName { get; }

        /// <summary>
        ///     Gets current User's BranchId or null.
        ///     It can be null if no user logged in.
        /// </summary>
        string BranchId { get; }

        /// <summary>
        ///     Gets current User's BranchName or null.
        ///     It can be null if no user logged in.
        /// </summary>
        string BranchName { get; }

        /// <summary>
        ///     Represents current user is from head-office branch.
        /// </summary>
        bool IsHeadBranch { get; }

        /// <summary>
        ///     Gets current user's Permissions
        ///     It can be null if no user logged in.
        /// </summary>
        IReadOnlyList<string> Permissions { get; }

        /// <summary>
        ///     Gets current user's Roles
        ///     It can be null if no user logged in.
        /// </summary>
        IReadOnlyList<string> Roles { get; }

        /// <summary>
        ///     Gets current user's Claims
        ///     It can be null if no user logged in.
        /// </summary>
        IReadOnlyList<Claim> Claims { get; }

        /// <summary>
        ///     Gets current UserDisplayName or null.
        ///     It can be null if no user logged in.
        /// </summary>
        string UserDisplayName { get; }

        /// <summary>
        ///     Gets current UserBrowserInfo or null.
        ///     It can be null if no user logged in.
        /// </summary>
        string UserBrowserName { get; }

        /// <summary>
        ///     Gets current UserIP or null.
        ///     It can be null if no user logged in.
        /// </summary>
        string UserIP { get; }

        /// <summary>
        ///     UserId of the impersonator.
        ///     This is filled if a user is performing actions behalf of the <see cref="UserId" />.
        /// </summary>
        string ImpersonatorUserId { get; }

        bool IsInRole(string role);
        bool IsGranted(string permission);
    }
}