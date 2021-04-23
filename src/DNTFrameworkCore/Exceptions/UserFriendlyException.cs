using System;

namespace DNTFrameworkCore.Exceptions
{
    [Serializable]
    public class UserFriendlyException : Exception
    {
        public UserFriendlyException(string message) : base(message)
        {
        }
    }
}