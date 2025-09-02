using framework;
using model.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using model;
using System.Linq;
using data;
using System.Text;
using framework.Security;
using framework.Extensions;
using api.Dtos;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private static int RequestCount = 0;
        private readonly UsuarioRepository UsuarioRepository;
        private readonly Repository<Pessoa> PessoaRepository;
        private readonly EnvioDeEmail EnvioDeEmail;
        private readonly Repository<RotinaDoSistema> RotinaDoSistemaRepository;
        private readonly IKeyProvider KeyProvider;
        private readonly Cfg Cfg;
        private readonly WebApiOperationResolver WebApiOperationResolver;
        private readonly Repository<CodigoDeSegurancaParaRecuperacaoDeSenha> CodigoDeSegurancaParaRecuperarSenhaRepository;
        private readonly IAppContext AppContext;
        private readonly TermosDeUsoRepository TermosDeUsoRepository;
        private readonly ParametroDoSistemaRepository ParametrosRepository;

        public LoginController(
                WebApiOperationResolver webApiOperationResolver,
                IAppContext appContext,
                Cfg cfg,
                EnvioDeEmail envioDeEmail,
                UsuarioRepository usuarioRepository,
                IKeyProvider keyProvider,
                Repository<RotinaDoSistema> rotinaDoSistemaRepository,
                Repository<Pessoa> pessoaRepository,
                Repository<CodigoDeSegurancaParaRecuperacaoDeSenha> codigoDeSegurancaParaRecuperarSenhaRepository,
                TermosDeUsoRepository termosDeUsoRepository,
                ParametroDoSistemaRepository parametrosRepository)
        {
            AppContext = appContext;
            Cfg = cfg;
            UsuarioRepository = usuarioRepository;
            PessoaRepository = pessoaRepository;
            EnvioDeEmail = envioDeEmail;
            RotinaDoSistemaRepository = rotinaDoSistemaRepository;
            KeyProvider = keyProvider;
            WebApiOperationResolver = webApiOperationResolver;
            CodigoDeSegurancaParaRecuperarSenhaRepository = codigoDeSegurancaParaRecuperarSenhaRepository;
            TermosDeUsoRepository = termosDeUsoRepository;
            ParametrosRepository = parametrosRepository;
        }
        
        [HttpPost("token")]
        public DadosDeLogin Token([FromBody] EncrytptedRequest encrytptedRequest)
        {
            var keyProvider = new api.WebApiKeyProvider();
            var privateKeyEncrypted = keyProvider.GetRSAPrivateKey().ToPlainText();
            var key = Encoding.UTF8.GetBytes(keyProvider.GetKey().ToPlainText());
            var iv = Encoding.UTF8.GetBytes(keyProvider.GetIV().ToPlainText());
            var privateKey = framework.Security.SymmetricEncryption.DecryptString(privateKeyEncrypted, key, iv);

            LoginRequest request = null;
            var validarRecaptcha = true;

            if (!string.IsNullOrWhiteSpace(encrytptedRequest.Jwe))
            {
                validarRecaptcha = false;
                var rsaParameters = default(RSAParameters);

                using (RSACryptoServiceProvider csp = new RSACryptoServiceProvider())
                {
                    csp.FromXmlString(privateKey);
                    rsaParameters = csp.ExportParameters(true);
                }

                var rsa = RSA.Create(rsaParameters);

                var decryptedText = Jose.JWE.Decrypt(encrytptedRequest.Jwe, rsa, Jose.JweAlgorithm.RSA_OAEP_256);
                request = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginRequest>(decryptedText.Plaintext);
            }
            else
            {

                var decrypted = framework.Security.AsymmetricEncryption.Decrypt(privateKey, encrytptedRequest.GetAsByteArray());
                var decryptedText = Encoding.UTF8.GetString(decrypted);
                request = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginRequest>(decryptedText);
            }

            request.NomeDeUsuario = !string.IsNullOrWhiteSpace(request.NomeDeUsuario) ?
                request.NomeDeUsuario.Contains("@") ? request.NomeDeUsuario.Trim().ToLower() : request.NomeDeUsuario.Trim() : "";
            RequestCount++;

            var login = request.NomeDeUsuario;

            var valid = Cfg.AccessControl.Authenticate(login, request.Senha, request.TipoDeAcesso);

            if (!valid)
            {
                throw new Exception("Usuário ou senha inválida.");
            }


            if (Cfg.Https && validarRecaptcha)
            {
                // ValidarRecaptcha(encrytptedRequest.Recaptcha);
            }

            var item = UsuarioRepository.GetUsuarioByLoginESenha(login, request.Senha, request.TipoDeAcesso);

            var role = request.TipoDeAcesso;

            var user = new User()
            {
                Id = item.Id,
                IsAuthenticated = true,
                AuthenticationType = "Custom",
                Name = request.NomeDeUsuario,
                Role = role,
                Domain = null,
                Origin = request.Origem
            };

            var token = Cfg.AccessControl.GenerateToken(user);
            var encryptedToken = framework.Security.SymmetricEncryption.EncryptString(
                token,
                Convert.FromBase64String(request.SymmetricKey.Key),
                Convert.FromBase64String(request.SymmetricKey.IV)
            );

            var result = DadosDeLogin.FromModel(
                item,
                Convert.ToBase64String(encryptedToken),
                role,
                request.Origem
            );

            result.RotinasAcessiveis = item.IsMaster() ?
                WebApiOperationResolver.RestrictOperations
                    .Select(i => new RotinaAcessivel()
                    {
                        Id = i.Key,
                    }).ToArray()
                :
                    item.GetAcessos()
                        .Select(i => new RotinaAcessivel()
                        {
                            Id = i.Rotina.Id,
                        }).ToArray();

            var loggedUser = new LoggedUser(item);

            AppContext.RegisterUserAction(loggedUser, UserAction.Login, "Login");

            return result;
        }

        static string ApplyCnpjMask(string cnpj)
        {
            if (cnpj.Length != 14 || !long.TryParse(cnpj, out _))
                return "Invalid CNPJ";

            return Regex.Replace(cnpj, @"(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})", "$1.$2.$3/$4-$5");
        }

        private void ValidarRecaptcha(string recaptcha)
        {
            WebRequestHelper.Execute(
                new CommunicationParameters()
                {
                    IgnoreCertificateErrors = false,
                    ReceiveTimeout = 20000,
                    SendTimeout = 20000,
                    UseDefaultProxy = true,
                    TlsSslVersion = System.Net.SecurityProtocolType.Tls12
                },
                new WebRequestParams()
                {
                    ContentType = "application/x-www-form-urlencoded",
                    Accept = "application/json",
                    Method = "POST",
                    Url = "https://www.google.com/recaptcha/api/siteverify"
                },
                new WebRequestHeaders(

                ),
                FormattedString.Build(
                    "secret", "=", Cfg.RecaptchaKey, "&",
                    "response", "=", recaptcha
                ),
                (response) =>
                {
                    /*
                    {
                    "success": true|false,
                    "challenge_ts": timestamp,  // timestamp of the challenge load (ISO format yyyy-MM-dd'T'HH:mm:ssZZ)
                    "hostname": string,         // the hostname of the site where the reCAPTCHA was solved
                    "error-codes": [...]        // optional
                    }
                    */
                    var responseObject = JObject.Parse(response);
                    var success = Boolean.Parse(responseObject["success"].ToString());
                    if (!success)
                    {
                        //throw new Exception("Recaptcha inválido.");
                    }
                }
            );
        }
        
        [HttpPost("recoverpassword")]
        public void Recover([FromBody] RecoverPasswordRequest request)
        {
            if (request.NomeDeUsuario == null)
            {
                throw new Exception("Usuário não informado.");
            }

            var usuario = UsuarioRepository.GetUsuarioByLogin(
                request.NomeDeUsuario,
                request.TipoDeAcesso);

            if (usuario == null)
            {
                throw new Exception("Usuário não identificado.");
            }

            ValidarRecaptcha(request.Recaptcha);

            var email = usuario.Email;

            var codigo = CreatePassword(8).ToUpperInvariant();

            var CodigoDeSeguranca = new CodigoDeSegurancaParaRecuperacaoDeSenha()
            {
                Codigo = codigo,
                HorarioDaEmissaoDoCodigo = DateTimeSync.Now,
                Usuario = usuario
            };

            CodigoDeSegurancaParaRecuperarSenhaRepository.Insert(CodigoDeSeguranca);

            try
            {
                if (!email.IsEmpty())
                {
                    EnvioDeEmail.Enviar(
                        ParametrosRepository,
                        "Recuperação de senha", email.GetPlainText(),
                        FormattedString.Build(
                        "Utilize esse código para recuperar sua senha",
                        "<br/><br/>",
                        "<b>", codigo.Substring(0, 3), " ", codigo.Substring(3, 3), " ", codigo.Substring(6, 3), "</b>"));
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Não foi possível enviar o email de recuperação de senha.\n\n{e.ToString()}");
            }

            AppContext.RegisterUserAction(UserAction.SolicitouCodigoParaRecuperacaoDeSenha, null);
        }
        
        [HttpPost("resetpassword")]
        public bool ConfirmRecover([FromBody] EncrytptedRequest encrytptedRequest)
        {
            var keyProvider = new api.WebApiKeyProvider();
            var privateKeyEncrypted = keyProvider.GetRSAPrivateKey().ToPlainText();
            var key = Encoding.UTF8.GetBytes(keyProvider.GetKey().ToPlainText());
            var iv = Encoding.UTF8.GetBytes(keyProvider.GetIV().ToPlainText());
            var privateKey = framework.Security.SymmetricEncryption.DecryptString(privateKeyEncrypted, key, iv);
            var decrypted = framework.Security.AsymmetricEncryption.Decrypt(privateKey, encrytptedRequest.GetAsByteArray());
            var decryptedText = Encoding.UTF8.GetString(decrypted);
            var request = Newtonsoft.Json.JsonConvert.DeserializeObject<ChangePasswordRequest>(decryptedText);

            request.NomeDeUsuario = !string.IsNullOrWhiteSpace(request.NomeDeUsuario) ?
                request.NomeDeUsuario.Contains("@") ?
                    request.NomeDeUsuario.Trim().ToLower() : request.NomeDeUsuario.Trim() : "";

            var usuario = UsuarioRepository.GetUsuarioByLogin(
                request.NomeDeUsuario,
                request.TipoDeAcesso);

            if (usuario == null)
            {
                throw new Exception("Usuário não encontrado.");
            }

            if (request.ConfirmacaoDaSenha != request.NovaSenha)
            {
                throw new Exception("A senha não confere com a confirmação da senha.");
            }
            request.CodigoDeSeguranca = request.CodigoDeSeguranca.Replace(" ", "");
            var CodigoDeSegurancaParaRecuperarSenha = new CodigoDeSegurancaParaRecuperacaoDeSenha();
            var codigoDeSeguranca = CodigoDeSegurancaParaRecuperarSenhaRepository.GetAll()
                .Where(i => i.Codigo == request.CodigoDeSeguranca)
                .Where(i => i.Usuario == usuario)
                .FirstOrDefault();
            if ((codigoDeSeguranca == null) ||
                (!CodigoDeSegurancaParaRecuperarSenha.CodigoDeSegurancaValido(codigoDeSeguranca)))
            {
                throw new Exception("Código exiprado.");
            }

            usuario.Senha = framework.EncryptedText.Build(FormattedString.Build(request.NovaSenha,
                Convert.ToBase64String(Encoding.UTF8.GetBytes(usuario.NomeDeUsuario))));
            UsuarioRepository.Update(usuario, usuario);

            AppContext.RegisterUserAction(UserAction.AlterouASenha, null);

            return true;
        }

        private string CreatePassword(int length)
        {
            var valid = "abcdefghkmnpqrstuvwxyzABCDEFGHKMNPQRSTUVWXYZ123456789";
            var res = new System.Text.StringBuilder();
            var rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
       
        [HttpPost("aceitartermosdeuso")]
        public void AceitarTermosDeUso()
        {
            var usuarioLogado = AppContext.Usuario();
            UsuarioRepository.AceitarTermosDeUso(usuarioLogado.Usuario.Id);
            var descricao = "Usuário aceitou os termos de uso.";
            AppContext.RegisterUserAction(UserAction.AceitouTermosDeUso, descricao);
        }
        
        [HttpGet("termosdeuso")]
        public virtual TermosDeUsoDoUsuarioResponse TermosDeUso()
        {
            var usuarioLogado = AppContext.Usuario();
            var usuario = UsuarioRepository.Get(usuarioLogado.Id, true);
            var termo = TermosDeUsoRepository.GetTermoDeUsoAtivo();
            var texto = "Termo de uso ainda não definido.";
            if (termo != null && !string.IsNullOrWhiteSpace(termo.Termo))
            {
                texto = termo.Termo;
            }
            var result = new TermosDeUsoDoUsuarioResponse()
            {
                JaAceitou = usuario.AceitouTermosDeUso,
                Termo = texto
            };
            return result;
        }

        public class DadosDeLogin
        {
            public long IdDoUsuario { get; set; }
            public string Token { get; set; }
            public string NomeDeUsuario { get; set; }
            public RotinaAcessivel[] RotinasAcessiveis { get; set; }
            public string PrimeiroNome { get; set; }
            public string NomeDoMeio { get; set; }
            public string UltimoNome { get; set; }
            public string NomeCompleto { get; set; }
            public ArquivoDto Foto { get; set; }
            public bool AceitouTermosDeUso { get; set; }
            public string TipoDeAcesso { get; set; }
            public string Origem { get; set; }
            public PaisDto Pais { get; set; }

            public static DadosDeLogin FromModel(Usuario item, string token, string tipoDeAcesso, string origem)
            {
                if (item == null)
                {
                    return null;
                }

                var result = new DadosDeLogin()
                {
                    IdDoUsuario = item.Id,
                    Token = token,
                    NomeDeUsuario = item.NomeDeUsuario,
                    NomeCompleto = item.Nome.GetPlainText(),
                    AceitouTermosDeUso = item.AceitouTermosDeUso,
                    TipoDeAcesso = tipoDeAcesso,
                    Origem = origem,
                    Foto = ArquivoDto.Build(item.Foto),
                };
                return result;
            }
        }

        public class EncrytptedRequest
        {
            public string[] Data { get; set; }
            public string Recaptcha { get; set; }
            public string Jwe { get; set; }

            public virtual byte[][] GetAsByteArray()
            {
                var result = new List<byte[]>();
                if (Data != null)
                {
                    Data.ForEach(i => result.Add(Convert.FromBase64String(i)));
                }
                return result.ToArray();
            }
        }

        public class LoginRequest
        {
            public string NomeDeUsuario { get; set; }
            public string Senha { get; set; }
            // ADM            
            public string TipoDeAcesso { get; set; }
            // WEB
            // MOBILE
            // DESKTOP
            public string Origem { get; set; }
            public SymmetricKey SymmetricKey { get; set; }
        }

        public class SymmetricKey
        {
            public string Key { get; set; }
            public string IV { get; set; }
        }

        public class RecoverPasswordRequest
        {
            // ADM
            public string TipoDeAcesso { get; set; }
            public string NomeDeUsuario { get; set; }
            public string Recaptcha { get; set; }
        }

        public class RotinaAcessivel
        {
            public long Id { get; set; }
        }

        public class ChangePasswordRequest
        {
            // ADM
            public string TipoDeAcesso { get; set; }
            public string NomeDeUsuario { get; set; }
            public string CodigoDeSeguranca { get; set; }
            public string NovaSenha { get; set; }
            public string ConfirmacaoDaSenha { get; set; }
        }

        public class AlterarIdiomaRequest
        {
            public string Idioma { get; set; }
        }

        public class TermosDeUsoDoUsuarioResponse
        {
            public string Termo { get; set; }
            public bool JaAceitou { get; set; }
        }
    }

    internal class LoggedUser : IAppUser
    {
        private Usuario _usuario;
        public long? Id => _usuario.Id;
        public long? RepresentingId => null;
        public long? UnitId => null;


        public LoggedUser(Usuario usuario)
        {
            _usuario = usuario;
        }
    }
}