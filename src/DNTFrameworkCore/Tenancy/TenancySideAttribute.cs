using System;

namespace DNTFrameworkCore.Tenancy
{
    /// <summary>
    /// Used to declare multi tenancy side of an object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method |
                    AttributeTargets.Interface)]
    public sealed class TenancySideAttribute : Attribute
    {
        public TenancySides Side { get; }

        public TenancySideAttribute(TenancySides side)
        {
            Side = side;
        }
    }
}