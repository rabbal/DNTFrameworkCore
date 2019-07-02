using System;
using System.Collections.Generic;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Sales
{
    public class Action : Entity
    {
        public Action(string name, ActionType type, string displayName)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(displayName)) throw new ArgumentNullException(nameof(displayName));
            
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Name = name;
            DisplayName = displayName;
        }

        public ActionType Type { get; private set; }
        public string Name { get; private set; }
        public string DisplayName { get; private set; }
    }
}