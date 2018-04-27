using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CustomerManager.Attributes
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private string _propertyName;
        private bool _propertyValue;

        public RequiredIfAttribute(string propertyName, bool propertyValue)
        {
            _propertyName = propertyName;
            _propertyValue = propertyValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var property = validationContext.ObjectInstance.GetType().GetProperty(_propertyName);
            var propertyValue = (bool)property.GetValue(validationContext.ObjectInstance, null);


            if (propertyValue && (value == null || (int)value == 0))
    
            {
                return new ValidationResult("This field is mandatory");
            }

            if (!propertyValue && value != null && (int)value != 0)

            {
                return new ValidationResult("Builsness customers are not allowed to have schools");
            }

            return ValidationResult.Success;
        }
    }
}