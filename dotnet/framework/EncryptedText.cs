using System.Text;
using framework.Security;
using framework.Extensions;

namespace framework
{
    public class EncryptedText
    {
        private string _value = null;
        private string _decryptedValue = null;
        private static byte[] _key = null;
        private static byte[] _iv = null;

        public static void SetKey(IKeyProvider keyProvider)
        {
            _key = UTF8Encoding.UTF8.GetBytes(keyProvider.GetKey().ToPlainText());
            _iv = UTF8Encoding.UTF8.GetBytes(keyProvider.GetIV().ToPlainText());
        }

        public EncryptedText()
        {
        }

        EncryptedText(string value)
        {
            if (value == string.Empty)
            {
                _value = string.Empty;
            }
            else
            if (value != null)
            {
                _value = System.Convert.ToBase64String(Security.SymmetricEncryption.EncryptString(value, _key, _iv));
            }
        }

        public void SetEncryptedText(string value)
        {
            _value = value;
        }

        public string GetEncryptedText()
        {
            var result = _value;
            return result;
        }

        public string GetDecryptedText()
        {
            var result = default(string);
            if (_decryptedValue != null)
            {
                result = _decryptedValue;
            }
            else
            if (!string.IsNullOrEmpty(_value))
            {
                _decryptedValue = Security.SymmetricEncryption.DecryptString(_value, _key, _iv);
            }
            return _decryptedValue;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(_value);
        }

        public bool CompareTo(string plainText)
        {
            var plainTextEncrypted = System.Text.Encoding.UTF8.GetString(
                Security.SymmetricEncryption.EncryptString(plainText, _key, _iv));
            var result = plainTextEncrypted == _value;
            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return _value != null ? _value.GetHashCode() : 0;
        }

        public static EncryptedText Build(string text)
        {
            if (text == null)
            {
                return null;
            }
            else
            {
                return new EncryptedText(text);
            }
        }
    }

    public static class EncryptedTextExtensions
    {
        public static string GetPlainText(this EncryptedText encryptedText)
        {
            if (encryptedText == null)
            {
                return null;
            }
            var result = encryptedText.GetDecryptedText();
            return result;
        }
    }
}
