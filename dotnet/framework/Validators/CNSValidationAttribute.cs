using System;

namespace framework.Validators
{
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public class CNSValidationAttribute : Attribute, IValidationAttribute
    {
        public string ErrorMessage { get { return "Cartão Nacional de Saúde inválido."; } }
        

        public bool IsValid(object value)
        {
            var result = value == null;
            if (!result)
            {
                if (typeof(string).IsAssignableFrom(value.GetType()))
                {
                    result = CNSValidator.chkNumeroCNS(value.ToString());
                }
                else
                if (typeof(EncryptedText).IsAssignableFrom(value.GetType()))
                {
                    result = CNSValidator.chkNumeroCNS(((EncryptedText)value).GetPlainText());
                }
            }
            return result;
        }
    }
}