using System.Linq;
using DNTFrameworkCore.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Authorization
{
    /// <summary>
    /// Most simple implementation of <see cref="IPermissionDependency"/>.
    /// It checks one or more permissions if they are granted.
    /// </summary>
    public class NamePermissionDependency : IPermissionDependency
    {
        /// <summary>
        /// A list of permissions to be checked if they are granted.
        /// </summary>
        public string[] Permissions { get; set; }

        /// <summary>
        /// If this property is set to true, all of the <see cref="Permissions"/> must be granted.
        /// If it's false, at least one of the <see cref="Permissions"/> must be granted.
        /// Default: false.
        /// </summary>
        public bool RequiresAll { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamePermissionDependency"/> class.
        /// </summary>
        /// <param name="permissions">The permissions.</param>
        public NamePermissionDependency(params string[] permissions)
        {
            Permissions = permissions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamePermissionDependency"/> class.
        /// </summary>
        /// <param name="requiresAll">
        /// If this is set to true, all of the <see cref="Permissions"/> must be granted.
        /// If it's false, at least one of the <see cref="Permissions"/> must be granted.
        /// </param>
        /// <param name="permissions">The permissions.</param>
        public NamePermissionDependency(bool requiresAll, params string[] permissions)
            : this(permissions)
        {
            RequiresAll = requiresAll;
        }

        public bool IsSatisfied(PermissionDependencyContext context)
        {
            var session = context.ServiceProvider.GetRequiredService<IUserSession>();
            return RequiresAll ? Permissions.All(session.IsGranted) : Permissions.Any(session.IsGranted);
        }
    }
}