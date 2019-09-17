using System;

namespace DNTFrameworkCore.Tenancy
{
    /// <summary>
    /// Used to declare multi tenancy side of an object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method |
                    AttributeTargets.Interface)]
    public class TenancySideAttribute : Attribute
    {
        public TenancySides Sides { get; }

        public TenancySideAttribute(TenancySides sides)
        {
            Sides = sides;
        }
    }
}