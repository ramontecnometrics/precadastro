using System;

namespace framework.Validators
{
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public class MinValueValidationAttribute : Attribute, IValidationAttribute
    {
        public string ErrorMessage { get; set; }
        public double MinValue { get; set; }

        public virtual bool IsValid(object value)
        {
            var result = true;
            var propValue = default(double?);

            if (value != null)
            {
                if (double.TryParse(value.ToString(), out double outValue))
                {
                    propValue = outValue;
                }
            }

            if (propValue.HasValue)
            {
                if ((MinValue > 0) && (propValue.Value < MinValue))
                {
                    result = false;
                }
            }
            return result;
        }
    }

  
    public class MaxValueValidationAttribute : Attribute, IValidationAttribute
    {
        public string ErrorMessage { get; set; }        
        public double MaxValue { get; set; }

        public virtual bool IsValid(object value)
        {
            var result = true;
            var propValue = default(double?);

            if (value != null)
            {
                if (double.TryParse(value.ToString(), out double outValue))
                {
                    propValue = outValue;
                }
            }

            if (propValue.HasValue)
            {
                if ((MaxValue > 0) && (propValue.Value > MaxValue))
                {
                    result = false;
                }
            }
            return result;
        }
    }
}
