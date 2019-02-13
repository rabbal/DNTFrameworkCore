using System.Collections.Generic;
using System.Collections.Immutable;
using DNTFrameworkCore.Extensions;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.Localization;
using DNTFrameworkCore.UI.Inputs;

namespace DNTFrameworkCore.Application.Features
{
    public class Feature
    {
        /// <summary>
        /// Gets/sets arbitrary objects related to this object.
        /// Gets null if given key does not exists.
        /// This is a shortcut for <see cref="Attributes"/> dictionary.
        /// </summary>
        /// <param name="key">Key</param>
        public object this[string key]
        {
            get => Attributes.GetOrDefault(key);
            set => Attributes[key] = value;
        }

        /// <summary>
        /// Arbitrary objects related to this object.
        /// These objects must be serializable.
        /// </summary>
        public IDictionary<string, object> Attributes { get; }

        /// <summary>
        /// Parent of this feature, if one exists.
        /// If set, this feature can be enabled only if parent is enabled.
        /// </summary>
        public Feature Parent { get; private set;}

        /// <summary>
        /// Unique name of the feature.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Display name of the feature.
        /// This can be used to show features on UI.
        /// </summary>
        public ILocalizableString DisplayName { get; set; }

        /// <summary>
        /// A brief description for this feature.
        /// This can be used to show feature description on UI. 
        /// </summary>
        public ILocalizableString Description { get; set; }

        /// <summary>
        /// Default value of the feature.
        /// This value is used if feature's value is not defined for current edition or tenant.
        /// </summary>
        public string DefaultValue { get; set; }

        public FeatureScopes Scopes { get; set; }

        /// <summary>
        /// Input type.
        /// This can be used to prepare an input for changing this feature's value.
        /// Default: <see cref="CheckboxInputType"/>.
        /// </summary>
        public IInputType InputType { get; set; }

        public IReadOnlyList<Feature> Children => _children.ToImmutableList();

        private readonly List<Feature> _children;

        public Feature(string name, string defaultValue, ILocalizableString displayName = null,
            ILocalizableString description = null, FeatureScopes scopes = FeatureScopes.All, IInputType inputType = null)
        {
            Guard.ArgumentNotNull(name, nameof(name));

            Name = name;
            DisplayName = displayName;
            Description = description;
            Scopes = scopes;
            DefaultValue = defaultValue;
            InputType = inputType ?? new CheckboxInputType();
            
            _children = new List<Feature>();
            Attributes = new Dictionary<string, object>();
        }

        public Feature CreateChildFeature(string name, string defaultValue, ILocalizableString displayName = null,
            ILocalizableString description = null)
        {
            var feature = new Feature(name, defaultValue, displayName, description) {Parent = this};
            _children.Add(feature);
            return feature;
        }
        
        public Feature Create(string name, string defaultValue, ILocalizableString displayName = null,
            ILocalizableString description = null)
        {
            return new Feature(name, defaultValue, displayName, description);
        }

        public override string ToString()
        {
            return $"[Feature: {Name}]";
        }
    }
}