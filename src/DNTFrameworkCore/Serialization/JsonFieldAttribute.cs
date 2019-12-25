using System;

namespace DNTFrameworkCore.Serialization
{
    /// <summary>
    /// Marks the property to be serialized into database as JSON.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonFieldAttribute : Attribute {}
}