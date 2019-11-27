using System.Collections.Generic;
using System.Collections.Immutable;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.Localization;
using DNTFrameworkCore.Tenancy;

namespace DNTFrameworkCore.Authorization
{
    /// <summary>
    /// Represents a permission.
    /// A permission is used to restrict functionalities of the application from unauthorized users.
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// Unique name of the permission.
        /// This is the key name to grant permissions.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Parent of this permission if one exists.
        /// If set, this permission can be granted only if parent is granted.
        /// </summary>
        public Permission Parent { get; private set; }

        /// <summary>
        /// Display name of the permission.
        /// This can be used to show permission to the user.
        /// </summary>
        public ILocalizableString DisplayName { get; }

        /// <summary>
        /// A brief description for this permission.
        /// </summary>
        public ILocalizableString Description { get; }

        /// <summary>
        /// Which side can use this permission.
        /// </summary>
        public TenancySides TenancySides { get; }

        /// <summary>
        /// Custom Properties. Use this to add your own properties to permission.
        /// <para>You can use this with indexer like Permission["mykey"]=data; </para>
        /// <para>object mydata=Permission["mykey"]; </para>
        /// </summary>
        public Dictionary<string, object> Properties { get; }

        /// <summary>
        /// Shortcut of Properties dictionary
        /// </summary>
        public object this[string key]
        {
            get => !Properties.ContainsKey(key) ? null : Properties[key];
            set => Properties[key] = value;
        }

        /// <summary>
        /// List of child permissions. A child permission can be granted only if parent is granted.
        /// </summary>
        public IReadOnlyList<Permission> Children => _children.ToImmutableList();

        private readonly List<Permission> _children;

        /// <summary>
        /// Creates a new Permission.
        /// </summary>
        /// <param name="name">Unique name of the permission</param>
        /// <param name="displayName">Display name of the permission</param>
        /// <param name="description">A brief description for this permission</param>
        /// <param name="properties">Custom Properties. Use this to add your own properties to permission.</param>
        /// <param name="tenancySides">Which side can use this permission</param>
        public Permission(
            string name,
            ILocalizableString displayName = null,
            ILocalizableString description = null,
            Dictionary<string, object> properties = null,
            TenancySides tenancySides = TenancySides.None)
        {
            Name = Ensure.IsNotNullOrEmpty(name, nameof(name));
            DisplayName = displayName;
            Description = description;
            TenancySides = tenancySides;
            Properties = properties ?? new Dictionary<string, object>();

            _children = new List<Permission>();
        }

        /// <summary>
        /// Adds a child permission.
        /// A child permission can be granted only if parent is granted.
        /// </summary>
        /// <returns>Returns newly created child permission</returns>
        public Permission CreateChildPermission(
            string name,
            ILocalizableString displayName = null,
            ILocalizableString description = null,
            Dictionary<string, object> properties = null,
            TenancySides tenancySides = TenancySides.None)
        {
            var permission = new Permission(name, displayName, description, properties, tenancySides)
                {Parent = this};
            _children.Add(permission);
            return permission;
        }

        public static Permission CreatePermission(
            string name,
            ILocalizableString displayName = null,
            ILocalizableString description = null,
            Dictionary<string, object> properties = null,
            TenancySides tenancySides = TenancySides.None
        )
        {
            var permission = new Permission(name, displayName, description, properties, tenancySides);

            return permission;
        }

        public void RemoveChildPermission(string name)
        {
            _children.RemoveAll(p => p.Name == name);
        }

        public override string ToString()
        {
            return $"[Permission: {Name}]";
        }
    }
}