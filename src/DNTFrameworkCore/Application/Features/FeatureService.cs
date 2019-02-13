using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Exceptions;
using DNTFrameworkCore.Extensions;
using DNTFrameworkCore.Functional;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Application.Features
{
    public interface IFeatureService : ISingletonDependency
    {
        Maybe<Feature> Find(string name);
        IReadOnlyList<Feature> ReadList();
    }
    
    internal class FeatureService : IFeatureService
    {
        private readonly IServiceProvider _provider;
        private readonly FeatureDictionary _features = new FeatureDictionary();


        public FeatureService(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));

            Initialize();
        }

        private void Initialize()
        {
            using (var scope = _provider.CreateScope())
            {
                var providers = scope.ServiceProvider.GetServices<IFeatureProvider>();

                foreach (var provider in providers)
                {
                    var features = provider.ProvideFeatures();
                    foreach (var feature in features)
                    {
                        if (_features.ContainsKey(feature.Name))
                        {
                            throw new FrameworkException("There is already a feature with name: " + feature.Name);
                        }

                        _features[feature.Name] = feature;
                    }
                }
            }

            _features.AddAllFeatures();
        }

        public Maybe<Feature> Find(string name)
        {
            return _features.GetOrDefault(name);
        }

        public IReadOnlyList<Feature> ReadList()
        {
            return _features.Values.ToImmutableList();
        }
    }
}