using System;

namespace framework.Validators
{
    [AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public class TelefoneCelularValidationAttribute : Attribute, IValidationAttribute
    {
        public string ErrorMessage { get { return "Telefone celular inválido."; } }

        public bool IsValid(object value)
        {
            var result = value == null;
            if (!result)
            {
                result = TelefoneCelularValidator.IsValid(value.ToString());
            }
            return result;
        }        
    }
}
