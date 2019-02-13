using System.Collections.Generic;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Application.Features
{
    public interface IFeatureProvider : ITransientDependency
    {
        IEnumerable<Feature> ProvideFeatures();
    }
}