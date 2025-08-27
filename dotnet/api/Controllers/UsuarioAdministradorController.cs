using System.Linq;
using framework;
using model;
using api.Dtos;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System;
using model.Repositories;
using framework.Extensions;
using data;
using System.Text;

namespace api.Controllers
{
    [Route("usuarioadministrador/[action]")]
    [Route("usuarioadministrador")]
    public class UsuarioAdministradorController :
           EntityFastController<
                 UsuarioAdministrador,
                 UsuarioAdministradorDto,
                 UsuarioAdministradorFast,
                 UsuarioAdministradorFastDto,
                 UsuarioAdministradorGetParams,
                 UsuarioAdministradorPostParams,
                 UsuarioAdministradorPutParams>
    {
        protected override UserAction UserActionForInsert => UserAction.InseriuUsuarioAdministrador;
        protected override UserAction UserActionForUpdate => UserAction.AlterouUsuarioAdministrador;
        protected override UserAction UserActionForDelete => UserAction.ExcluiuUsuarioAdministrador;

        private readonly UsuarioAdministradorRepository UsuarioAdministradorRepository;
        private readonly Repository<Arquivo> ArquivoRepository;        
        private readonly PerfilDeUsuarioRepository PerfilDeUsuarioRepository;
        private readonly EnvioDeEmail EnvioDeEmail;        
        private readonly ParametroDoSistemaRepository ParametroDoSistemaRepository;

        public UsuarioAdministradorController(
            UsuarioAdministradorRepository repository,
            UsuarioAdministradorFastRepository fastRepository,
            Repository<Arquivo> arquivoRepository,
            Repository<Cidade> cidadeRepository,
            PerfilDeUsuarioRepository perfilDeUsuarioRepository,
            EnvioDeEmail envioDeEmail,
            Repository<EnderecoDePessoa> enderecoDePessoaRepository,
            IAppContext appContext,
            ParametroDoSistemaRepository parametroDoSistemaRepository) :
            base(repository, fastRepository, appContext)
        {
            UsuarioAdministradorRepository = repository;
            ArquivoRepository = arquivoRepository;            
            PerfilDeUsuarioRepository = perfilDeUsuarioRepository;
            EnvioDeEmail = envioDeEmail;            
            ParametroDoSistemaRepository = parametroDoSistemaRepository;
        }

        protected override IQueryable<UsuarioAdministrador> Get(UsuarioAdministradorGetParams getParams)
        {
            var result = Repository.GetAll()
                .Where(i => i.Situacao != SituacaoDeUsuario.Excluido);
            if (getParams.Id.HasValue)
            {
                result = result.Where(i => i.Id == getParams.Id.Value);
            }
            if (!string.IsNullOrEmpty(getParams.Searchable))
            {
                result = result.Where(i => i.Searchable.Contains(getParams.Searchable));
            }

            return result;
        }

        protected override IQueryable<UsuarioAdministradorFast> FastGet(UsuarioAdministradorGetParams getParams)
        {
            var result = FastRepository.GetAll();
            if (getParams.Id.HasValue)
            {
                result = result.Where(i => i.Id == getParams.Id.Value);
            }
            if (!string.IsNullOrEmpty(getParams.Searchable))
            {
                result = result.Where(i => i.Searchable.Contains(getParams.Searchable));
            }
            return result;
        }

        protected override UsuarioAdministradorDto Convert(UsuarioAdministrador entity)
        {
            var result = UsuarioAdministradorDto.Build(entity);
            return result;
        }

        protected override UsuarioAdministradorFastDto Convert(UsuarioAdministradorFast entity)
        {
            var result = UsuarioAdministradorFastDto.Build(entity);
            return result;
        }

        protected override UsuarioAdministrador Convert(UsuarioAdministradorPostParams insertRequest)
        {
            var usuario = new UsuarioAdministrador()
            {
                Email = EncryptedText.Build(insertRequest.Email),
                NomeDeUsuario = insertRequest.NomeDeUsuario,
                Nome = EncryptedText.Build(insertRequest.Nome),
                Certificado = EncryptedText.Build(insertRequest.Certificado),
                Situacao = SituacaoDeUsuario.Ativo,
                Perfis = new List<PerfilDoUsuario>(),
            };

            if ((insertRequest.Perfis == null) || (insertRequest.Perfis != null && !insertRequest.Perfis.Any()))
            {
                throw new Exception("Informe o perfil de usuário.");
            }

            insertRequest.Perfis.ForEach(perfil =>
            {
                if (perfil.Perfil.Nome == "MASTER")
                {
                    throw new Exception("Somente o usuário MASTER pode usar o perfil MASTER.");
                }
                usuario.Perfis.Add(
                    new PerfilDoUsuario()
                    {
                        Perfil = PerfilDeUsuarioRepository.Get(perfil.Perfil.Id, true),
                        Usuario = usuario
                    }
                );
            });

            var decryptedText = string.Empty;
            if (insertRequest.SenhaAlterada)
            {
                if (!string.IsNullOrEmpty(insertRequest.NovaSenha))
                {
                    var keyProvider = new api.WebApiKeyProvider();
                    var privateKeyEncrypted = keyProvider.GetRSAPrivateKey().ToPlainText();
                    var key = Encoding.UTF8.GetBytes(keyProvider.GetKey().ToPlainText());
                    var iv = Encoding.UTF8.GetBytes(keyProvider.GetIV().ToPlainText());
                    var privateKey = framework.Security.SymmetricEncryption.DecryptString(privateKeyEncrypted, key, iv);
                    var decrypted = framework.Security.AsymmetricEncryption.Decrypt(privateKey,
                        System.Convert.FromBase64String(insertRequest.NovaSenha));
                    decryptedText = Encoding.UTF8.GetString(decrypted);
                    usuario.Senha = framework.EncryptedText.Build(FormattedString.Build(decryptedText,
                        System.Convert.ToBase64String(Encoding.UTF8.GetBytes(usuario.NomeDeUsuario))));
                }
            }

            if (insertRequest.EnviarNovaSenhaPorEmail)
            {
                EnvioDeEmail.EnviarEmailDeNovaSenha(ParametroDoSistemaRepository, insertRequest.Email, decryptedText);
            }

            usuario.Email = EncryptedText.Build(insertRequest.Email);

            if (insertRequest.Foto != null)
            {
                usuario.Foto = ArquivoRepository.Get(insertRequest.Foto.Id, true);
            }
            ;


            return usuario;
        }

        protected override UsuarioAdministrador Convert(UsuarioAdministradorPutParams updateRequest,
            UsuarioAdministrador oldUsuarioAdministrador)
        {
            oldUsuarioAdministrador.Situacao = updateRequest.Situacao;
            oldUsuarioAdministrador.Email = EncryptedText.Build(updateRequest.Email);
            oldUsuarioAdministrador.Nome = EncryptedText.Build(updateRequest.Nome);
            oldUsuarioAdministrador.Certificado = EncryptedText.Build(updateRequest.Certificado);            
            oldUsuarioAdministrador.Foto = updateRequest.Foto != null 
                ? ArquivoRepository.Get(updateRequest.Foto.Id, true) 
                : null;

            if ((updateRequest.Perfis == null) || (updateRequest.Perfis != null && !updateRequest.Perfis.Any()))
            {
                throw new Exception("Informe o perfil de usuário.");
            }

            if (oldUsuarioAdministrador.Perfis == null)
            {
                oldUsuarioAdministrador.Perfis = new List<PerfilDoUsuario>();
            }

            oldUsuarioAdministrador.Perfis.Merge(
                updateRequest.Perfis,
                (i, j) => i.Perfil.Id == j.Perfil.Id,
                (i) =>
                {
                    oldUsuarioAdministrador.Perfis.Remove(i);
                    UsuarioAdministradorRepository.Delete(i);
                },
                null,
                (j) =>
                    oldUsuarioAdministrador.Perfis.Add(
                    new PerfilDoUsuario()
                    {
                        Perfil = PerfilDeUsuarioRepository.Get(j.Perfil.Id, true),
                        Usuario = oldUsuarioAdministrador
                    })
            );

            oldUsuarioAdministrador.Perfis.ForEach(i =>
            {
                if (i.Perfil.Nome == "MASTER")
                {
                    throw new Exception("Somente o usuário MASTER pode usar o perfil MASTER.");
                }
            });

            var decryptedText = string.Empty;
            if (updateRequest.SenhaAlterada)
            {
                if (!string.IsNullOrEmpty(updateRequest.NovaSenha))
                {
                    var keyProvider = new api.WebApiKeyProvider();
                    var privateKeyEncrypted = keyProvider.GetRSAPrivateKey().ToPlainText();
                    var key = Encoding.UTF8.GetBytes(keyProvider.GetKey().ToPlainText());
                    var iv = Encoding.UTF8.GetBytes(keyProvider.GetIV().ToPlainText());
                    var privateKey = framework.Security.SymmetricEncryption.DecryptString(privateKeyEncrypted, key, iv);
                    var decrypted = framework.Security.AsymmetricEncryption.Decrypt(privateKey,
                        System.Convert.FromBase64String(updateRequest.NovaSenha));
                    decryptedText = Encoding.UTF8.GetString(decrypted);
                    oldUsuarioAdministrador.Senha = framework.EncryptedText.Build(FormattedString.Build(decryptedText,
                        System.Convert.ToBase64String(Encoding.UTF8.GetBytes(oldUsuarioAdministrador.NomeDeUsuario))));
                }
                else
                {
                    oldUsuarioAdministrador.Senha = null;
                }
            }

            if (updateRequest.EnviarNovaSenhaPorEmail)
            {
                EnvioDeEmail.EnviarEmailDeNovaSenha(ParametroDoSistemaRepository, updateRequest.Email, decryptedText);
            }

            return oldUsuarioAdministrador;
        }

    }

    public class UsuarioAdministradorGetParams : IId
    {
        public long? Id { get; set; }
        public string Searchable { get; set; }
        public string Cpf { get; set; }
        public string Cnpj { get; set; }
    }

    public class UsuarioAdministradorPostParams : IPostParams
    {
        public string Email { get; set; }
        public Tipo<SituacaoDeUsuario> Situacao { get; set; }
        public ArquivoDto Foto { get; set; }
        public string Nome { get; set; }
        public bool SenhaAlterada { get; set; }
        public string NovaSenha { get; set; }
        public string NomeDeUsuario { get; set; }
        public IEnumerable<PerfilDoUsuarioDto> Perfis { get; set; }
        public bool EnviarNovaSenhaPorEmail { get; set; }
        public string Certificado { get; set; }
    }

    public class UsuarioAdministradorPutParams : IPutParams
    {
        public long? Id { get; set; }
        public string Email { get; set; }
        public Tipo<SituacaoDeUsuario> Situacao { get; set; }
        public ArquivoDto Foto { get; set; }
        public string Nome { get; set; }
        public bool SenhaAlterada { get; set; }
        public string NovaSenha { get; set; }
        public string NomeDeUsuario { get; set; }
        public IEnumerable<PerfilDoUsuarioDto> Perfis { get; set; }
        public bool EnviarNovaSenhaPorEmail { get; set; }
        public string Certificado { get; set; }
    }
}