using System.ComponentModel.DataAnnotations;

namespace DNTFrameworkCore.Helpers
{
    public static class ValidationHelper
    {
        public static bool TryValidateObject(this object instance)
        {
            return Validator.TryValidateObject(instance, new ValidationContext(instance, null, null), null);
        }
    }
}