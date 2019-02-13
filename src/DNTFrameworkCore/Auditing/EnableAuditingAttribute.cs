using System;

namespace DNTFrameworkCore.Auditing
{
    /// <summary>
    /// This attribute is used to apply audit logging for a single method or
    /// all methods of a class or interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class EnableAuditingAttribute : Attribute
    {
    }
}
