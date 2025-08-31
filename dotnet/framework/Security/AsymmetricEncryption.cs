using System.Security.Cryptography;
using System.Collections.Generic;
using framework.Extensions;
using System;

namespace framework.Security
{
    public static class AsymmetricEncryption
    {
        public static byte[] PublicEncrypt(string publicKey, byte[] dataToEncrypt)
        {
            byte[] result;
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                result = rsa.Encrypt(dataToEncrypt, false);
            }
            return result;
        }

        public static byte[] Decrypt(string privateKey, byte[][] data)
        {
            var decrypted = new List<byte[]>();
            data.ForEach(i => decrypted.Add(Decrypt(privateKey, i)));
            var result = new List<byte>();
            decrypted.ForEach(i => result.AddRange(i));
            return result.ToArray();
        }

        public static byte[] Decrypt(string privateKey, byte[] data)
        {
            var rsaParameters = default(RSAParameters);

            using (RSACryptoServiceProvider csp = new RSACryptoServiceProvider())
            {
                csp.FromXmlString(privateKey);
                rsaParameters = csp.ExportParameters(true);
            }

            var rsa = RSA.Create(rsaParameters);

            var result = new byte[] { };
            try
            {
                result = rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao descriptografar os dados. Tente logar novamente. \n\nErro: {e.Message}\n\nDados: {Convert.ToBase64String(data)}", e);
            }
            return result;
        }
    }
}
