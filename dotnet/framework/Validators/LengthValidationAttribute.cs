using System;

namespace framework.Validators
{
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public class LengthValidationAttribute : Attribute, IValidationAttribute
    {
        public string ErrorMessage { get; set; }

        public int MaxLength { get; set; }
        public int MinLength { get; set; }
        public string MinLengthMessage { get; set; }
        public string MaxLengthMessage { get; set; }

        public virtual bool IsValid(object value)
        {
            var result = string.IsNullOrWhiteSpace(GetErrorMessage(value));
            return result;
        }

        protected virtual string GetErrorMessage(object value)
        {
            var propValue = string.Empty;
            var result = string.Empty;

            if (value != null)
            {
                if (typeof(EncryptedText).IsAssignableFrom(value.GetType()))
                {
                    propValue = ((EncryptedText)value).GetPlainText();
                }
                else
                {
                    propValue = value.ToString();
                }
            }
            if (propValue.Length > 0)
            {
                if (MinLength > 0)
                {
                    if ((string.IsNullOrWhiteSpace(propValue)) ||
                        (propValue.Length < MinLength))
                    {
                        result = MinLengthMessage;
                    }
                }

                if (MaxLength > 0)
                {
                    if ((!string.IsNullOrWhiteSpace(propValue)) &&
                        (propValue.Length > MaxLength))
                    {
                        result = MaxLengthMessage;
                    }
                }
            }
            return result;
        }

        public void Validate(object value)
        {
            var message = GetErrorMessage(value);
            if (!string.IsNullOrWhiteSpace(message))
            {
                throw new Exception(message);
            }
        }
    }
}
