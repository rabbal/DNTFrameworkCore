using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Dependency
{
    public static class ServiceDescriptorExtensions
    {
        public static Type GetImplementationType(this ServiceDescriptor descriptor)
        {
            if (descriptor is ClonedSingletonDescriptor cloned)
            {
                // Use the parent descriptor as it was before being cloned.
                return cloned.Parent.GetImplementationType();
            }

            if (descriptor.ImplementationType != null)
            {
                return descriptor.ImplementationType;
            }

            if (descriptor.ImplementationInstance != null)
            {
                return descriptor.ImplementationInstance.GetType();
            }

            return descriptor.ImplementationFactory?.GetType().GetTypeInfo().GenericTypeArguments[1];
        }
    }
}