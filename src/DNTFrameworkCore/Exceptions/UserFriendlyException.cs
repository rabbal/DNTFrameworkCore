using System;

namespace DNTFrameworkCore.Exceptions
{
    [Serializable]
    public class UserFriendlyException : FrameworkException
    {
        public UserFriendlyException(string message) : base(message)
        {
        }
    }
}