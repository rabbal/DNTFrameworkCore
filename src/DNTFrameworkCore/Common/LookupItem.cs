using System;

namespace DNTFrameworkCore.Common
{
    /// <summary>
    /// Can be used to store Text/Value pairs.
    /// </summary>
    [Serializable]
    public class LookupItem : LookupItem<string>
    {
    }

    /// <summary>
    /// Can be used to store Text/Value pairs.
    /// </summary>
    [Serializable]
    public class LookupItem<TValue>
    {
        public string Text { get; set; }
        public TValue Value { get; set; }
    }
}