using System;

namespace DNTFrameworkCore.UI.Inputs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InputTypeAttribute : Attribute
    {
        public string Name { get; set; }

        public InputTypeAttribute(string name)
        {
            Name = name;
        }
    }
}