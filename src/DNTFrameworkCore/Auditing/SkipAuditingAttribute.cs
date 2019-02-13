using System;

namespace DNTFrameworkCore.Auditing
{
    /// <summary>
    /// Used to disable auditing for a single method or
    /// all methods of a class or interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class SkipAuditingAttribute : Attribute
    {
    }
}