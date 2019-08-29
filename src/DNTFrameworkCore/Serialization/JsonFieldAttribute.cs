using System;

namespace DNTFrameworkCore.Serialization
{
    /// <summary>
    /// Marks the property to be serialized into database as JSON.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class JsonFieldAttribute : Attribute {}
}