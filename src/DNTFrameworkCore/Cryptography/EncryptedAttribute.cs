using System;

namespace DNTFrameworkCore.Cryptography
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class EncryptedAttribute : Attribute
    { }
}