using System.Security;
using framework.Security;
using System.Text;
using framework.Extensions;

namespace api
{
    public class WebApiKeyProvider : IKeyProvider
    {
        private static string _RSAPublicKey;
        private static SecureString _RSAPrivateKey;        

        public SecureString GetKey()
        {
            // Importante sempre alterar esse quando trocar de aplicação.
            var result = new SecureString();
            result.AppendChar('0');
            result.AppendChar('6');
            result.AppendChar('5');
            result.AppendChar('6');
            result.AppendChar('0');
            result.AppendChar('b');
            result.AppendChar('a');
            result.AppendChar('7');
            result.AppendChar('3');
            result.AppendChar('d');
            result.AppendChar('3');
            result.AppendChar('g');
            result.AppendChar('2');
            result.AppendChar('6');
            result.AppendChar('4');
            result.AppendChar('f');
            result.AppendChar('1');
            result.AppendChar('b');
            result.AppendChar('a');
            result.AppendChar('5');
            result.AppendChar('e');
            result.AppendChar('9');
            result.AppendChar('3');
            result.AppendChar('1');
            result.AppendChar('5');
            result.AppendChar('4');
            result.AppendChar('c');
            result.AppendChar('f');
            result.AppendChar('2');
            result.AppendChar('c');
            result.AppendChar('a');
            result.AppendChar('a');
            result.MakeReadOnly();
            return result;
        }

        public SecureString GetIV()
        {
            // Importante sempre alterar esse quando trocar de aplicação.
            var result = new SecureString();
            result.AppendChar('0');
            result.AppendChar('6');
            result.AppendChar('5');
            result.AppendChar('6');
            result.AppendChar('0');
            result.AppendChar('b');
            result.AppendChar('a');
            result.AppendChar('7');
            result.AppendChar('3');
            result.AppendChar('d');
            result.AppendChar('3');
            result.AppendChar('g');
            result.AppendChar('2');
            result.AppendChar('6');
            result.AppendChar('4');
            result.AppendChar('f');
            result.MakeReadOnly();     
            return result;
        }

        public SecureString GetRSAPrivateKey()
        {
            if (_RSAPrivateKey == null) {
                var encryptedKey = Cfg.RSAPrivateKeyEncrypted;
                if (string.IsNullOrEmpty(encryptedKey))
                {
                    throw new System.Exception("A chave privada não foi informada. Criptografe a chave privada em formato XML e coloque no arquivo appsettings.json na opção RSAPrivateKeyEncrypted.");
                }
                var key = SymmetricEncryption.DecryptString(encryptedKey, 
                    UTF8Encoding.UTF8.GetBytes(GetKey().ToPlainText()), 
                    UTF8Encoding.UTF8.GetBytes(GetIV().ToPlainText()));
                _RSAPrivateKey = ConvertToSecureString(encryptedKey);
            }
            return _RSAPrivateKey;
        }

        public string GetRSAPublicKey()
        {                           
            if (string.IsNullOrEmpty(Cfg.RSAPublicKey))
            {
                throw new System.Exception("A chave pública não foi informada. Coloque a chave pública em formato XML no arquivo appsettings.json na opção RSAPublicKey.");
            }
            _RSAPublicKey = Cfg.RSAPublicKey;          
            return _RSAPublicKey;
        }

        private SecureString ConvertToSecureString(string text)
        {
            var securePassword = new SecureString();
            foreach (char c in text)
                securePassword.AppendChar(c);

            securePassword.MakeReadOnly();
            return securePassword;
        }
    }
}