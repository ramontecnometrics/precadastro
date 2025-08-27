using System;

namespace framework.Validators
{
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public class CPFValidationAttribute : Attribute, IValidationAttribute
    {
        public string ErrorMessage { get { return "CPF inválido."; } }        

        public bool IsValid(object value)
        {
            var result = value == null;
            if (!result)
            {
                if (typeof(string).IsAssignableFrom(value.GetType()))
                {
                    result = CPFValidator.IsValid(value.ToString());
                } 
                else                
                if (typeof(EncryptedText).IsAssignableFrom(value.GetType()))
                {
                    result = CPFValidator.IsValid(((EncryptedText)value).GetPlainText());
                }
            }
            return result;
        }
    }
}
