// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Security.Claims;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.MultiTenancy;

namespace DNTFrameworkCore.Runtime
{
    /// <summary>
    /// Defines some session information that can be useful for applications.
    /// </summary>
    public interface IUserSession : IScopedDependency
    {
        bool IsAuthenticated { get; }

        /// <summary>
        /// Gets current UserId or null.
        /// It can be null if no user logged in.
        /// </summary>
        long? UserId { get; }

        /// <summary>
        /// Gets current UserName or null.
        /// It can be null if no user logged in.
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// Gets current User's BranchId or null.
        /// It can be null if no user logged in.
        /// </summary>
        long? BranchId { get; }
        
        /// <summary>
        /// Gets current User's BranchNumber or null.
        /// It can be null if no user logged in.
        /// </summary>
        string BranchNumber { get; }

        /// <summary>
        /// Gets current user's Permissions
        /// It can be null if no user logged in.
        /// </summary>
        IReadOnlyList<string> Permissions { get; }

        /// <summary>
        /// Gets current user's Roles
        /// It can be null if no user logged in.
        /// </summary>
        IReadOnlyList<string> Roles { get; }

        /// <summary>
        /// Gets current user's Claims
        /// It can be null if no user logged in.
        /// </summary>
        IReadOnlyList<Claim> Claims { get; }

        /// <summary>
        /// Gets current UserDisplayName or null.
        /// It can be null if no user logged in.
        /// </summary>
        string UserDisplayName { get; }

        /// <summary>
        /// Gets current UserBrowserInfo or null.
        /// It can be null if no user logged in.
        /// </summary>
        string UserBrowserName { get; }

        /// <summary>
        /// Gets current UserIP or null.
        /// It can be null if no user logged in.
        /// </summary>
        string UserIP { get; }

        /// <summary>
        /// Gets current TenantId or null.
        /// This TenantId should be the TenantId of the <see cref="UserId"/>.
        /// It can be null if given <see cref="UserId"/> is a host user or no user logged in.
        /// </summary>
        long? TenantId { get; }

        /// <summary>
        /// Gets current multi-tenancy side.
        /// </summary>
        MultiTenancySides MultiTenancySide { get; }

        /// <summary>
        /// UserId of the impersonator.
        /// This is filled if a user is performing actions behalf of the <see cref="UserId"/>.
        /// </summary>
        long? ImpersonatorUserId { get; }

        /// <summary>
        /// TenantId of the impersonator.
        /// This is filled if a user with <see cref="ImpersonatorUserId"/> performing actions behalf of the <see cref="UserId"/>.
        /// </summary>
        long? ImpersonatorTenantId { get; }

        bool IsInRole(string role);
        bool IsGranted(string permission);
    }
}