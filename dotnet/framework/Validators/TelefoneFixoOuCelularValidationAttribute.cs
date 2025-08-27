using System;

namespace framework.Validators
{
    [AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public class TelefoneFixoOuCelularValidationAttribute : Attribute, IValidationAttribute
    {
        public string ErrorMessage { get { return "Telefone fixo inválido."; } }

        public bool IsValid(object value)
        {
            var result = value == null;
            if (!result)
            {
                result = TelefoneFixoValidator.IsValid(value.ToString());
                if (!result)
                {
                    result = TelefoneCelularValidator.IsValid(value.ToString());
                }
            }
            return result;
        }
    }
}
