using System.Text.RegularExpressions;

namespace framework.Validators
{
    public static class TelefoneFixoValidator
    {
        public static bool IsValid(string numeroDeTelefone)
        {
            var result = false;
            var pattern = @"^\([1-9][0-9]\)[1-9]\d{3}-\d{4}$";
            var regex = new Regex(pattern);
            if (!string.IsNullOrWhiteSpace(numeroDeTelefone))
            {
                result = regex.IsMatch(numeroDeTelefone);
            }
            return result;
        }
    }
}
