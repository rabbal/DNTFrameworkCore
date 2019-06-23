using System;

namespace DNTFrameworkCore.MultiTenancy
{
    /// <summary>
    /// Used to declare multi tenancy side of an object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method |
                    AttributeTargets.Interface)]
    public class MultiTenancySideAttribute : Attribute
    {
        public MultiTenancySides Side { get; }

        public MultiTenancySideAttribute(MultiTenancySides side)
        {
            Side = side;
        }
    }
}