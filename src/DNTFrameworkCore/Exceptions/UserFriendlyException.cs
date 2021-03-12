using System;

namespace DNTFrameworkCore.Exceptions
{
    [Serializable]
    public class UserFriendlyException : DNTException
    {
        public UserFriendlyException(string message) : base(message)
        {
        }
    }
}