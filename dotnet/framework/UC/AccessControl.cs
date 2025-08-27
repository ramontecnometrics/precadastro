using System;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using framework.Security;
using framework.Extensions;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IO;
using System.Buffers.Text;

[assembly: InternalsVisibleTo("Testes")]
[assembly: InternalsVisibleTo("api")]

namespace framework
{
    public class User
    {
        public virtual long? Id { get; set; }
        public virtual long? RepresentingId { get; set; }
        public string AuthenticationType { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Domain { get; set; }
        public string Origin { get; set; }
        public Authentication Authentication { get; set; }

        public ClaimsPrincipal ToClaimsPrincipal()
        {
            var result = new UserClaimsPrincipal(this);

            return result;
        }
    }

    public class UserClaimsPrincipal : ClaimsPrincipal
    {
        public long? Id { get; set; }
        public long? RepresentingId { get; set; }

        public UserClaimsPrincipal(User user)
        {
            this.Id = user.Id;
            this.RepresentingId = user.RepresentingId;
        }
    }

    public class Authentication
    {
        public DateTime Date { get; set; }
        public string RequestToken { get; set; }
        public string TokenUID { get; set; }
        public AuthenticationStatus Status { get; set; }
    }

    public enum AuthenticationStatus
    {
        InvalidUser,
        AuthenticationNotRequired,
        Authenticated,
    }

    public enum AccessValidationStatus
    {
        AccessDenied,
        AccessGranted,
        BlockedContract,
        FinishedContract,
    }

    public enum TokenValidationStatus
    {
        Unknown,
        InvalidUser,
        Expired,
        Valid,
        Overwriten,
    }

    public interface IOperation
    {
        long Id { get; }
        string Description { get; }
        bool RequiresAuthentication { get; }
        bool OpenOperation { get; set; }
        bool RestrictedOperation { get; set; }
    }

    public interface IOperationResolver
    {
        IOperation Resolve(string httpMethod, string httpUrl);
    }

    public interface IUserValidator
    {
        bool IsValid(string userId, string password, string role);
        bool CanExecuteOperation(User user, IOperation operation);
        int GetExpireTimeInSeconds();
        void SetLastValidToken(string userId, string origin, string tokenUid);
        string GetLastValidTokenUID(string userId, string origin);
        string GetContractStatus(string userId, string origin);
        string GetPublicKey(string entity, string userName);
        long? GetUserId(string entity, string userName);
    }

    public class NotAuthenticatedException : Exception
    {
        public NotAuthenticatedException(string message)
            : base(message)
        {

        }
    }

    public class AccessDeniedException : Exception
    {
        public AccessDeniedException(string message)
            : base(message)
        {

        }
    }

    public class AccessControl
    {
        public IUserValidator UserValidator { get; set; }
        public IKeyProvider KeyProvider { get; set; }

        public AccessControl(IUserValidator userValidator, IKeyProvider keyProvider)
        {
            UserValidator = userValidator;
            KeyProvider = keyProvider;
        }

        private static bool IsJwt(string stringValues)
        {
            var result = false;
            string value = stringValues;

            if (!string.IsNullOrEmpty(value) && value.Split(".").Length == 3)
            {
                result = true;
            }

            return result;
        }

        private static byte[] Base64UrlDecode(string input)
        {
            string base64 = input.Replace("-", "+").Replace("_", "/");
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        private static RSA JwkToRsa(string jwkJson)
        {
            var jwk = JsonConvert.DeserializeObject<JsonWebKey>(jwkJson);
            var rsa = RSA.Create();
            rsa.ImportParameters(new RSAParameters
            {
                Modulus = Base64UrlDecode(jwk.N),
                Exponent = Base64UrlDecode(jwk.E)
            });
            return rsa;
        }


        public virtual User ValidadeCredentials(long? userId, string authorization, IOperation operation)
        {
            User user = null;

            if (!userId.HasValue && IsJwt(authorization))
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(authorization);
                string entity = token.Issuer;
                string userName = token.Subject;

                userId = UserValidator.GetUserId(entity, userName);

                if (!userId.HasValue)
                {
                    throw new Exception("Usuário inválido ou sem chave pública cadastrada.");
                }

                var publicKey = UserValidator.GetPublicKey(entity, userName);                

                if (string.IsNullOrEmpty(publicKey))
                {
                    throw new Exception("Usuário sem chave pública cadastrada.");
                }
                user = ValidateJwt(authorization, publicKey);
                user.Id = userId;
            }
            else
            if (userId.HasValue)
            {
                byte[] Key = UTF8Encoding.UTF8.GetBytes(KeyProvider.GetKey().ToPlainText());
                byte[] IV = UTF8Encoding.UTF8.GetBytes(KeyProvider.GetIV().ToPlainText());

                var data = SymmetricEncryption.DecryptString(authorization, Key, IV);
                TokenValidationStatus status = TokenValidationStatus.Unknown;
                try
                {
                    user = JsonConvert.DeserializeObject<User>(data);
                    if (!userId.Equals(user.Id))
                    {
                        status = TokenValidationStatus.InvalidUser;
                    }
                    else
                    if (DateTime.UtcNow >= user.Authentication.Date.AddSeconds(UserValidator.GetExpireTimeInSeconds()))
                    {
                        status = TokenValidationStatus.Expired;
                    }
                    else
                    {
                        status = TokenValidationStatus.Valid;
                    }
                    var lastValidTokenUID = UserValidator.GetLastValidTokenUID(userId.Value.ToString(), user.Origin);
                    if (string.IsNullOrWhiteSpace(lastValidTokenUID))
                    {
                        status = TokenValidationStatus.Expired;
                    }
                }
                catch (Exception)
                {
                    throw new NotAuthenticatedException("Invalid authorization data.");
                }

                switch (status)
                {
                    case TokenValidationStatus.Valid: break;
                    case TokenValidationStatus.Unknown: throw new Exception("Erro de desconhecido no processo de autenticação.");
                    case TokenValidationStatus.InvalidUser: throw new Exception("Usuário inválido.");
                    case TokenValidationStatus.Expired: throw new Exception("Sessão expirada.");
                    case TokenValidationStatus.Overwriten: throw new Exception("Sessão finalizada. Outra sessão foi aberta para esse login.");
                    default: break;
                }
            }

            if (user == null)
            {
                if (!operation.RequiresAuthentication)
                {
                    user = new User()
                    {
                        Id = null,
                        Name = "Unknown",
                        AuthenticationType = "None",
                        IsAuthenticated = false,
                        Authentication = new Authentication()
                        {
                            Date = DateTimeSync.Now,
                            Status = AuthenticationStatus.AuthenticationNotRequired,
                            RequestToken = null,
                            TokenUID = null
                        },
                    };
                }
                else
                {
                    throw new Exception("Usuário inválido.");
                }
            }

            return user;
        }

        private User ValidateJwt(string jwt, string publicKey)
        {
            var result = default(User);
            try
            {
                var rsa = default(RSA);
                var kid = default(string);

                if (publicKey.StartsWith("{"))
                {
                     rsa = JwkToRsa(publicKey);
                }
                else
                if (publicKey.StartsWith("-----BEGIN PUBLIC KEY-----"))
                {
                    rsa = ReadRsaPublicKeyFromPem(publicKey);
                }
                else
                if (publicKey.StartsWith("-----BEGIN CERTIFICATE-----"))
                {
                    rsa = ReadRsaPublicKeyFromCertificatePem(publicKey);
                }

                var publicKeyBytes = rsa.ExportSubjectPublicKeyInfo();                

                // Calcule o hash SHA-256 da chave pública.
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hash = sha256.ComputeHash(publicKeyBytes);
                    kid = ToBase64Url(hash);
                }

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new RsaSecurityKey(rsa),
                    ValidateIssuer = false,  // Ajuste conforme necessário
                    ValidateAudience = false, // Ajuste conforme necessário
                    ValidateLifetime = true, // Ativa a verificação de expiração
                    ClockSkew = TimeSpan.Zero, // Remove qualquer tolerância de horário
                };

                var handler = new JwtSecurityTokenHandler();

                var token = handler.ReadJwtToken(jwt);             

                // https://www.epochconverter.com/
                DateTime now = DateTimeSync.Now.ToUniversalTime();
                DateTime expDateTime = DateTimeOffset.FromUnixTimeSeconds(token.Payload.Expiration.Value).DateTime;
                DateTime nbfDateTime = DateTimeOffset.FromUnixTimeSeconds(token.Payload.NotBefore.Value).DateTime;
                DateTime iatDateTime = token.Payload.IssuedAt;

                if (now > expDateTime)
                {
                    throw new Exception($"Token expirado em {expDateTime.ToString("yyyy/MM/dd HH:mm:ss")} GMT");
                }

                if (expDateTime > now.AddDays(1))
                {
                    throw new Exception($"A data de validade do token não pode ultrapassar 24h. Gere um novo token com uma nova data de expiração no campo \"exp\" até {now.AddDays(1).ToString("yyyy/MM/dd HH:mm:ss")} GMT");
                }

                if (now < nbfDateTime)
                {
                    throw new Exception($"Esse token só será válido em {nbfDateTime.ToString("yyyy/MM/dd HH:mm:ss")} GMT");
                }

                if (kid != token.Header.Kid)
                {
                    throw new Exception($"Esse token foi assinado com chave diferente. Verifique o campo \"kid\", ele deve ser o SHA-256 da chave.");
                }

                if (token.Header.Alg != "PS256")
                {
                    throw new Exception($"Esse token foi assinado com um algoritmo diferente. Use PS256 na assinatura.");
                }

                handler.ValidateToken(jwt, validationParameters, out var validatedToken);

                result = new User()
                {
                    Id = null,
                    Name = token.Subject,
                    AuthenticationType = "Jwt",
                    IsAuthenticated = true,
                    Authentication = new Authentication()
                    {
                        Date = now,
                        Status = AuthenticationStatus.Authenticated,
                        RequestToken = null,
                        TokenUID = null
                    },
                };

            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao validar o JWT:\n\n{e.Message}");
            }

            return result;
        }

        public static string ToBase64Url(byte[] input)
        {
            string base64 = Convert.ToBase64String(input);
            base64 = base64.Replace('+', '-').Replace('/', '_');
            base64 = base64.TrimEnd('=');
            return base64;
        }

        public static RSA ReadRsaPublicKeyFromPem(string pem)
        {
            var base64 = new StringBuilder(pem)
                .Replace("-----BEGIN PUBLIC KEY-----", "")
                .Replace("-----END PUBLIC KEY-----", "")
                .Replace("\n", "")
                .Replace("\r", "")
                .ToString();

            var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(base64), out _);
            return rsa;
        }

        public static RSA ReadRsaPublicKeyFromCertificatePem(string pem)
        {
            var base64 = new StringBuilder(pem)
                .Replace("-----BEGIN CERTIFICATE-----", "")
                .Replace("-----END CERTIFICATE-----", "")
                .Replace("\n", "")
                .Replace("\r", "")
                .ToString();

            byte[] certData = Convert.FromBase64String(base64);
            var certificate = new X509Certificate2(certData);
            RSA rsa = certificate.GetRSAPublicKey();
            return rsa;
        }

        public virtual AccessValidationStatus CanExecuteOperation(User user, IOperation operation)
        {
            var result = AccessValidationStatus.AccessDenied;

            if (UserValidator.CanExecuteOperation(user, operation))
            {
                result = AccessValidationStatus.AccessGranted;
            }

            if (user.IsAuthenticated)
            {
                var contractStatus = UserValidator.GetContractStatus(user.Id.ToString(), user.Origin);
                if (contractStatus == "BLOQUEADO")
                {
                    result = AccessValidationStatus.BlockedContract;
                }
                else
                if (contractStatus == "ENCERRADO")
                {
                    result = AccessValidationStatus.FinishedContract;
                }
            }
            return result;
        }

        public virtual bool Authenticate(string userId, string password, string role)
        {
            var result = UserValidator.IsValid(userId, password, role);
            return result;
        }

        public string GenerateToken(User user)
        {
#pragma warning disable SYSLIB0022 // Type or member is obsolete
            using RijndaelManaged myRijndael = new RijndaelManaged();
#pragma warning restore SYSLIB0022 // Type or member is obsolete
            var token = Convert.ToBase64String(myRijndael.Key);
            user.Authentication = new Authentication()
            {
                Date = DateTimeSync.Now,
                RequestToken = token,
                TokenUID = Guid.NewGuid().ToString(),
                Status = AuthenticationStatus.Authenticated,
            };
            byte[] Key = UTF8Encoding.UTF8.GetBytes(KeyProvider.GetKey().ToPlainText());
            byte[] IV = UTF8Encoding.UTF8.GetBytes(KeyProvider.GetIV().ToPlainText());
            var text = Newtonsoft.Json.JsonConvert.SerializeObject(user);
            var data = framework.Security.SymmetricEncryption.EncryptString(text, Key, IV);
            UserValidator.SetLastValidToken(user.Id.Value.ToString(), user.Origin, user.Authentication.TokenUID);
            return Convert.ToBase64String(data); ;
        }
    }
}