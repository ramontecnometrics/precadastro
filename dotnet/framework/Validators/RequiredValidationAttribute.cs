using System;

namespace framework.Validators
{
    [AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredValidationAttribute : Attribute, IGroupValidationAtribute
    {
        public string ErrorMessage { get; set; }
        public string GroupId { get; set; }
        public GroupValidationType GroupValidationType { get; set; }

        public RequiredValidationAttribute()
        {

        }    

        public RequiredValidationAttribute(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public RequiredValidationAttribute(string errorMessage, string groupId,
            GroupValidationType groupValidationType)
        {
            ErrorMessage = errorMessage;
            GroupId = groupId;
            GroupValidationType = groupValidationType;
        }

        public bool IsValid(object value)
        {
            var result = value != null;
            if (result)
            {
                if (typeof(string).IsAssignableFrom(value.GetType()))
                {
                    result = !string.IsNullOrWhiteSpace((string)value);
                } 
                else
                if (typeof(DateTime).IsAssignableFrom(value.GetType()))
                {
                    result = (DateTime)value != DateTime.MinValue;
                }
                else
                if (typeof(TimeSpan).IsAssignableFrom(value.GetType()))
                {
                    result = ((TimeSpan)value).TotalSeconds > 0;
                }
                else
                if (typeof(EncryptedText).IsAssignableFrom(value.GetType()))
                {
                    result = !string.IsNullOrWhiteSpace(((EncryptedText)value).GetPlainText());
                }                
            }
            return result;
        }
    }
}
