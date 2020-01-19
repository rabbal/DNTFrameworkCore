using System;

namespace DNTFrameworkCore.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class SkipNormalizationAttribute : Attribute
    {
    }
}