using System.Security;

namespace framework.Security
{
    public interface IKeyProvider
    {
        SecureString GetKey();
        SecureString GetIV();
        SecureString GetRSAPrivateKey();
        string GetRSAPublicKey();
    }
}