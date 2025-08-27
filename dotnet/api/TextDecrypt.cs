using System.Text;
using framework;
using framework.Extensions;

namespace api
{
    public class TextDecrypt 
    {
        public static string GetDecryptedText(string text) 
        {
            var field = new EncryptedText();
            field.SetEncryptedText(text);
            return field.GetDecryptedText();
        }
    }
}