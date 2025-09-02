using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using framework.Security;
using framework.Extensions;
using System.Diagnostics;
using api;
using System.Security.Cryptography;
using System;

namespace tests
{
    [TestClass]
    public class EncryptDataUtil
    {
        const string linha = "=============================================================";

        [TestMethod]
        public void GenerateRsaKeyPair()
        {
            // Gere um par de chaves RSA público/privada
            var rsa = new RSACryptoServiceProvider(2048);
            string privateKey = rsa.ToXmlString(true);
            string publicKey = rsa.ExportSubjectPublicKeyInfoPem();
            Debug.WriteLine(linha);
            Debug.WriteLine("GenerateRsaKeyPair()");

            // Criptografe o XML usando a função TestEncryptRsaPrivateKey().
            // Depois cole no campo "RSAPrivateKeyEncrypted" do appsettings.config
            Debug.WriteLine("Private Key XML: ");
            Debug.WriteLine(privateKey);
            Debug.WriteLine("");

            // Cole no campo "RSAPublicKey" do appsettings.config
            // Como é um json substitua as quebras de linha por \n antes de colar.
            Debug.WriteLine("Public Key: ");
            Debug.WriteLine(publicKey);
            Debug.WriteLine(linha);

        }

        [TestMethod]        
        public void TestEncryptRsaPrivateKey()
        {
            // A chave privada RSA usada na transmissão de dados pela api é criptografada e fica no appsettings.config

            // Cole o XML da chave privada aqui
            var rsaPrivateKeyXml = "<RSAKeyValue><Modulus>s...";

            // No WebApiKeyProvider tem a chave de criptografia de toda aplicação. É uma chave simétrica que permite
            // Criptografar e descriptogravar senhas, demais dados protegidos que ficam no banco.            
            var KeyProvider = new WebApiKeyProvider();
            var key = Encoding.UTF8.GetBytes(KeyProvider.GetKey().ToPlainText());
            var iv = Encoding.UTF8.GetBytes(KeyProvider.GetIV().ToPlainText());
            var encrypted = System.Convert.ToBase64String(SymmetricEncryption.EncryptString(rsaPrivateKeyXml, key, iv));
            var decrypted = SymmetricEncryption.DecryptString(encrypted, key, iv);
            Assert.AreEqual(decrypted, rsaPrivateKeyXml);

            Debug.WriteLine(linha);
            Debug.WriteLine("TestEncryptRsaPrivateKey()");
            Debug.WriteLine("Private Key Encrypted: " + encrypted);
            Debug.WriteLine(linha);
        }

        [TestMethod]       
        public void TestEncryptText()
        {
            // Senha a ser criptografada
            var plainText = "";

            var KeyProvider = new WebApiKeyProvider();
            var key = Encoding.UTF8.GetBytes(KeyProvider.GetKey().ToPlainText());
            var iv = Encoding.UTF8.GetBytes(KeyProvider.GetIV().ToPlainText());
            var encrypted = System.Convert.ToBase64String(SymmetricEncryption.EncryptString(plainText, key, iv));
            var decrypted = SymmetricEncryption.DecryptString(encrypted, key, iv);
            Assert.AreEqual(decrypted, plainText);
            Debug.WriteLine(linha);
            Debug.WriteLine("TestEncryptText()");
            Debug.WriteLine("Plain text: " + plainText);
            Debug.WriteLine("Encrypted text: " + encrypted);            
            Debug.WriteLine(linha);            
        }
        
        [TestMethod]
        public void TestDecryptText()
        {
            // Cole aqui a senha criptografada
            var encrypted = "";

            var KeyProvider = new WebApiKeyProvider();
            var key = Encoding.UTF8.GetBytes(KeyProvider.GetKey().ToPlainText());
            var iv = Encoding.UTF8.GetBytes(KeyProvider.GetIV().ToPlainText());
            var decrypted = SymmetricEncryption.DecryptString(encrypted, key, iv);
            Debug.WriteLine(linha);
            Debug.WriteLine("TestDecryptText()");
            Debug.WriteLine("Encrypted text: " + encrypted); 
            Debug.WriteLine("Decrypted text: " + decrypted);
            Debug.WriteLine(linha);
        }
    }
}
