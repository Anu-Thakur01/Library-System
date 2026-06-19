using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace LibrarySystem.Shared.Attributes
{
    public class CompareValuesAttribute : ValidationAttribute
    {
        private readonly string _otherProperty;

        public CompareValuesAttribute(string otherProperty)
        {
            _otherProperty = otherProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Get current property value
            var currentValue = value;

            // Get other property info
            PropertyInfo otherPropertyInfo =
                validationContext.ObjectType.GetProperty(_otherProperty);

            if (otherPropertyInfo == null)
            {
                return new ValidationResult($"Property '{_otherProperty}' not found.");
            }

            // Get other property value
            var otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance);






            // Compare values
            if (!object.Equals(currentValue, otherValue))
            {
                return new ValidationResult(ErrorMessage ??
                    $"{validationContext.DisplayName} must match {_otherProperty}");
            }

            return ValidationResult.Success;
        }
    }
}



