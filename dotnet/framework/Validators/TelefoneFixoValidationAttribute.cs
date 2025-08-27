using System;

namespace framework.Validators
{
    [AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public class TelefoneFixoValidationAttribute : Attribute, IValidationAttribute
    {
        public string ErrorMessage { get; set; }

        public bool IsValid(object value)
        {
            var result = value == null;
            if (!result)
            {
                result = TelefoneFixoValidator.IsValid(value.ToString());
            }
            return result;
        }
    }
}
