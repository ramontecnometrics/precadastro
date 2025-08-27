using framework;
using System.IO;

namespace api
{
    // Scoped
    public class Cfg
    {
        // Gerados a cada requisição
        public AccessControl AccessControl { get; set; }
        public IOperationResolver OperationResolver { get; set; }

        // Gerados quando o sistema inicia (Startup.cs)
        public static string FileStoragePath { get; set; }
        public static string RSAPrivateKeyEncrypted { get; set; }
        public static string RSAPublicKey { get; set; }
        public static bool RunServices { get; set; }
        public static bool Https { get; set; }
        public static bool LogSql { get; set; }
        public static bool LogPackages { get; set; }
        public static string PublicApiUrl { get; set; }
        public static string PdfBuilderUrl { get; set; }        
        public string RecaptchaKey { get;  set; }

        public Cfg(AccessControl accessControl, IOperationResolver operationResolver)
        {
            AccessControl = accessControl;
            OperationResolver = operationResolver;
        }

        public static void Validate()
        {
            // FileStoragePath
            if (string.IsNullOrEmpty(FileStoragePath))
            {
                throw new System.Exception(
                    FormattedString.Build(
                    "Diretório para armazenamento de arquivos não foi definido.",
                    "\n",
                    "Informe o parâmetro \"FileStoragePath\" no arquivo appsettings.json."));
            }
            if (!Directory.Exists(FileStoragePath))
            {
                throw new System.Exception(
                    FormattedString.Build(
                    "Diretório para armazenamento de arquivos não existe.",
                    "\n",
                    "Verifique se o diretório ",
                    FileStoragePath,
                    " existe ou altere o parâmetro \"FileStoragePath\" no arquivo appsettings.json."));
            }

            // RSAPrivateKeyEncrypted
            if (string.IsNullOrEmpty(RSAPrivateKeyEncrypted))
            {
                throw new System.Exception(
                    FormattedString.Build(
                    "A chave privada para usada para criptografia assimétrica não foi definida.",
                    "\n",
                    "Informe o parâmetro \"RSAPrivateKeyEncrypted\" no arquivo appsettings.json."));
            }

            // RSAPublicKey
            if (string.IsNullOrEmpty(RSAPublicKey))
            {
                throw new System.Exception(
                    FormattedString.Build(
                    "A chave pública para usada para criptografia assimétrica não foi definida.",
                    "\n",
                    "Informe o parâmetro \"RSAPublicKey\" no arquivo appsettings.json."));
            }
        }
    }
}
