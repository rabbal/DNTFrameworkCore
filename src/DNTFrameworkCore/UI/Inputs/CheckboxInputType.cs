using System;
using DNTFrameworkCore.Validation;

namespace DNTFrameworkCore.UI.Inputs
{
    [Serializable]
    [InputType("CHECKBOX")]
    public class CheckboxInputType : InputTypeBase
    {
        public CheckboxInputType()
            : this(new BooleanValueValidator())
        {
            
        }

        public CheckboxInputType(IValueValidator validator)
            : base(validator)
        {
            
        }
    }
}