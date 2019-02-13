using System;

namespace DNTFrameworkCore.Validation
{
    [Validator("NULL")]
    [Serializable]
    public class AlwaysValidValueValidator : ValueValidatorBase
    {
        public override bool IsValid(object value)
        {
            return true;
        }
    }
}