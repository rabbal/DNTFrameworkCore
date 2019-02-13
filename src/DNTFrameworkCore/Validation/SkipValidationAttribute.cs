using System;

namespace DNTFrameworkCore.Validation
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class )]
    public sealed class SkipValidationAttribute : Attribute
    {
    }
}