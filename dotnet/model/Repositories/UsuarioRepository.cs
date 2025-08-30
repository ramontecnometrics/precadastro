using data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using framework;

namespace model.Repositories
{
    public class UsuarioRepository : Repository<Usuario>
    {
        private readonly Repository<AcessoDePerfilDeUsuario> AcessoDePerfilDeUsuarioRepository;
        private readonly Repository<RotinaDoSistema> RotinaDoSistemaRepository;
        private readonly Repository<UsuarioAdministrador> UsuarioAdministradorRepository;
        private readonly PerfilDeUsuarioRepository PerfilDeUsuarioRepository;
        private readonly Repository<PerfilDoUsuario> PerfilDoUsuarioRepository;
        private readonly object _lock = new object();
        private readonly ISqlCommand SqlCommand;

        public UsuarioRepository(
            Repository<AcessoDePerfilDeUsuario> acessoDePerfilDeUsuarioRepository,
            Repository<RotinaDoSistema> rotinaDoSistemaRepository,
            PerfilDeUsuarioRepository perfilDeUsuarioRepository,
            Repository<UsuarioAdministrador> usuarioAdministradorRepository,
            ISqlCommand sqlCommand,
            IUnitOfWork<Usuario> unitOfWork,
                        Repository<PerfilDoUsuario> perfilDoUsuarioRepository) :
            base(unitOfWork)
        {
            AcessoDePerfilDeUsuarioRepository = acessoDePerfilDeUsuarioRepository;
            RotinaDoSistemaRepository = rotinaDoSistemaRepository;
            PerfilDeUsuarioRepository = perfilDeUsuarioRepository;
            UsuarioAdministradorRepository = usuarioAdministradorRepository;
            SqlCommand = sqlCommand;
            PerfilDoUsuarioRepository = perfilDoUsuarioRepository;
        }

        public override Usuario Get(long id)
        {
            return UnitOfWork.Get(id);
        }

        public virtual Usuario GetUsuarioByLoginESenha(string login, string senha, string role)
        {
            if (string.IsNullOrEmpty(senha))
            {
                return null;
            }

            login = login.Contains("@") ? login.ToLower().Trim() : login.Trim();

            if (!(new string[] { Roles.ADM }.Contains(role)))
            {
                throw new Exception($"Role inválido: {role}");
            }

            Usuario result = null;

            if (login == "MASTER")
            {
                var senhaEncriptada = framework.EncryptedText.Build(FormattedString
                    .Build(senha, Convert.ToBase64String(Encoding.UTF8.GetBytes(login))));
                var master = UnitOfWork.GetAll()
                    .Where(p => p.NomeDeUsuario == login)
                    .Where(p => p.Senha == senhaEncriptada)
                    .Where(p => p.Situacao == SituacaoDeUsuario.Ativo)
                    .FirstOrDefault();
                if (master == null)
                {
                    master = UnitOfWork.GetAll()
                        .Where(p => p.NomeDeUsuario == "MASTER")
                        .FirstOrDefault();

                    if (master == null)
                    {
                        master = new UsuarioAdministrador()
                        {
                            Nome = EncryptedText.Build("MASTER"),
                            NomeDeUsuario = "MASTER",
                            Senha = senhaEncriptada,
                            Situacao = SituacaoDeUsuario.Ativo
                        };
                        var perfilMaster = GetPerfilMaster();
                        master.Perfis = new List<PerfilDoUsuario>();
                        master.Perfis.Add(new PerfilDoUsuario()
                        {
                            Perfil = perfilMaster,
                            Usuario = master,
                        });
                        UnitOfWork.Insert(master);
                    }
                    else
                    if (master.Senha != senhaEncriptada)
                    {
                        master = null;
                    }
                }
                result = master;
            }
            else
            {
                if (role == Roles.ADM)
                {
                    var senhaEncriptada = framework.EncryptedText.Build(FormattedString
                        .Build(senha, Convert.ToBase64String(Encoding.UTF8.GetBytes(login))));
                    var usuarios = UsuarioAdministradorRepository.GetAll()
                        .Where(p => p.NomeDeUsuario == login)
                        .Where(p => p.Senha == senhaEncriptada)
                        .Where(p => p.Situacao == SituacaoDeUsuario.Ativo);

                    if (usuarios.Count() > 1)
                    {
                        throw new Exception("Usuário duplicado: " + login);
                    }

                    result = usuarios.FirstOrDefault();
                }
            }

            // Impedir o login se não tiver nenhum perfil ativo
            if (result != null)
            {
                if (!result.Perfis.Any(perfilDeUsuario =>
                    perfilDeUsuario.Perfil.Situacao == SituacaoDePerfilDeUsuario.Ativo))
                {
                    result = null;
                }
            }

            return result;
        }

        public string GetContractStatus(string idDoUsuario, string origin)
        {
            var result =  "ATIVO";
            return result;
        }

        protected virtual PerfilDeUsuario GetPerfilMaster()
        {
            var perfil = PerfilDeUsuarioRepository.GetPerfilByNome("MASTER");
            if (perfil == null)
            {
                perfil = new PerfilDeUsuario()
                {
                    Nome = "MASTER",
                    Situacao = SituacaoDePerfilDeUsuario.Ativo,
                    TipoDePerfil = TipoDePerfilDeUsuario.Administrativo
                };
                PerfilDeUsuarioRepository.Insert(perfil);
            }
            return perfil;
        }

        public virtual Usuario GetUsuarioByLogin(string login, string role)
        {
            login = login.Contains("@") ? login.ToLower().Trim() : login.Trim();
            Usuario result = null;

            if (role == Roles.ADM)
            {
                var usuarios = UsuarioAdministradorRepository.GetAll()
                    .Where(p => p.NomeDeUsuario == login)
                    .Where(p => p.Situacao == SituacaoDeUsuario.Ativo);

                if (usuarios.Count() > 1)
                {
                    throw new Exception("Usuário duplicado: " + login);
                }

                result = usuarios.FirstOrDefault();
            }

            // Verificar se tem perfil ativo
            if (result != null)
            {
                if (!result.Perfis.Any(perfilDeUsuario =>
                    perfilDeUsuario.Perfil.Situacao == SituacaoDePerfilDeUsuario.Ativo))
                {
                    result = null;
                }
            }

            return result;
        }

        public virtual bool TemAcesso(long idDoUsuario, long? impersonalizacao, long rotina)
        {
            var result = false;

            var isMaster = false;            

            isMaster = PerfilDoUsuarioRepository.GetAll()
                .Where(i => i.Usuario.Id == idDoUsuario && i.Perfil.Nome == "MASTER" && i.Perfil.TipoDePerfil == TipoDePerfilDeUsuario.Administrativo)
                .Count() > 0;
             
            if (isMaster)
            {
                result = true;
            }
            else
            if (rotina >= 9000)
            {
                result = true;
            }
            else
            {
                var aceitouTermosDeUso = GetAll().Where(i => i.Id == idDoUsuario && i.AceitouTermosDeUso).Count() > 0;

                // if (aceitouTermosDeUso)
                {

                    var ativo = PerfilDoUsuarioRepository.GetAll()
                        .Where(i => i.Usuario.Id == idDoUsuario &&
                                i.Usuario.Situacao == SituacaoDeUsuario.Ativo &&
                                i.Perfil.TipoDePerfil == TipoDePerfilDeUsuario.Administrativo && 
                                i.Perfil.Situacao == SituacaoDePerfilDeUsuario.Ativo)
                        .Count() > 0;

                    // Se estiver ativo verifica se tem a permissão
                    if (ativo)
                    {
                        var cmd = new CommandData();
                        cmd.Sql = @"select count(1) qtde
  from perfildeusuario
  join acessodeperfildeusuario
    on acessodeperfildeusuario.perfildeusuario_id = perfildeusuario.id
  join perfildousuario
    on perfildousuario.perfil_id = perfildeusuario.id   
 where perfildeusuario.situacao = 1
   and rotina_id = @rotina_id
   and usuario_id = @usuario_id";

                        cmd.AddParametter("rotina_id", rotina);
                        cmd.AddParametter("usuario_id", idDoUsuario);

                        var record = SqlCommand.Execute(cmd).FirstOrDefault();

                        if (record != null)
                        {
                            result = record.GetFieldValue<int>("qtde") > 0;
                        }
                    }
                }
            }
            return result;
        }

        public virtual string GetLastValidTokenUID(string userId, string origin)
        {
            var command = new CommandData();
            if (origin == "WEB")
            {
                command.Sql = @"select LastValidTokenUidForWeb as LastValidTokenUid from usuario where id = @id";
            }
            else
            if (origin == "MOBILE")
            {
                command.Sql = @"select LastValidTokenUidForMobile as LastValidTokenUid from usuario where id = @id";
            }
            else
            if (origin == "DESKTOP")
            {
                command.Sql = @"select LastValidTokenUidForDesktop as LastValidTokenUid from usuario where id = @id";
            }
            else
            {
                throw new Exception("Origem do acesso não definida.");
            }

            command.AddParametter("id", int.Parse(userId));
            var resultSet = SqlCommand.Execute(command);
            string result = null;
            if (resultSet.Any())
            {
                result = resultSet.First().GetFieldValue<string>("LastValidTokenUid");
            }
            return result;
        }

        public virtual void SetLastValidToken(string userId, string origin, string tokenUid)
        {
            var command = new CommandData();
            if (origin == "WEB")
            {
                command.Sql = @"update usuario set LastValidTokenUidForWeb = @LastValidTokenUid, thumbprint = md5(cast(now() as text)) where id = @id";
            }
            else if (origin == "MOBILE")
            {
                command.Sql = @"update usuario set LastValidTokenUidForMobile = @LastValidTokenUid, thumbprint = md5(cast(now() as text)) where id = @id";
            }
            else
            if (origin == "DESKTOP")
            {
                command.Sql = @"update usuario set LastValidTokenUidForDesktop = @LastValidTokenUid, thumbprint = md5(cast(now() as text)) where id = @id";
            }
            else
            {
                throw new Exception("Origem do acesso não definida.");
            }

            command.AddParametter("id", int.Parse(userId));
            command.AddParametter("LastValidTokenUid", tokenUid);
            SqlCommand.ExecuteNonQuery(command);
        }

        public override void Insert(Usuario item)
        {
            throw new InvalidOperationException("Método inválido para inclusão de usuário.");
        }

        public virtual void Insert(Usuario item, Usuario usuarioLogado)
        {
            if (item.NomeDeUsuario != null)
            {
                item.NomeDeUsuario = item.NomeDeUsuario.Trim();
            }
            var itemSalvo = GetUsuarioByLogin(item.NomeDeUsuario);
            if (itemSalvo != null)
            {
                if (item.NomeDeUsuario.Contains("@"))
                {
                    throw new Exception("Já existe um usuário cadastrado com esse e-mail.");
                }
                else
                {
                    throw new Exception("Já existe um usuário cadastrado com esse login.");
                }
            }
            base.Validate(item);
            item.Searchable = item.GetSearchableText();
            UnitOfWork.Insert(item);
        }

        protected virtual Usuario GetUsuarioByLogin(string login)
        {
            login = login.Contains("@") ? login.ToLower().Trim() : login.Trim();
            var result = UnitOfWork.GetAll()
                .Where(p => p.NomeDeUsuario == login)
                .FirstOrDefault();
            return result;
        }

        public override void Update(Usuario usuario)
        {
            throw new InvalidOperationException("Método inválido para alteração de usuário.");
        }

        public virtual void Update(Usuario usuario, Usuario usuarioLogado)
        {
            usuario.Searchable = usuario.GetSearchableText();
            UnitOfWork.Update(usuario);
        }

        public override void Delete(Usuario usuario)
        {
            usuario.Situacao = SituacaoDeUsuario.Excluido;
            UnitOfWork.Update(usuario);
        }

        protected virtual bool EstaCriptografada(string senha)
        {
            if (string.IsNullOrEmpty(senha))
                return false;
            return senha.Length == 64;
        }

        public virtual void AceitarTermosDeUso(long? userId)
        {
            var command = new CommandData();
            command.Sql = @"update usuario set aceitoutermosdeuso = @aceitoutermosdeuso, thumbprint = md5(cast(now() as text)) where id = @id";
            command.AddParametter("id", userId);
            command.AddParametter("aceitoutermosdeuso", true);
            SqlCommand.ExecuteNonQuery(command);
        }

        public virtual void ValidarInclusaoDeUsuario(UsuarioAdministrador usuario)
        {
            var usuarios = UsuarioAdministradorRepository.GetAll()
                .Where(p => p.NomeDeUsuario == usuario.NomeDeUsuario)
                .Where(p => p.Situacao == SituacaoDeUsuario.Ativo);

            if (usuarios.Count() > 0)
            {
                throw new Exception($"Usuário já existe: {usuario.NomeDeUsuario}");
            }
        }

        public virtual void ValidarAlteracaoDeUsuario(UsuarioAdministrador usuario)
        {
            if (usuario.Situacao.Is(SituacaoDeUsuario.Ativo))
            {
                var usuarios = UsuarioAdministradorRepository.GetAll()
                    .Where(p => p.NomeDeUsuario == usuario.NomeDeUsuario)
                    .Where(p => p.Situacao == SituacaoDeUsuario.Ativo)
                    .Where(p => p.Id != usuario.Id);

                if (usuarios.Count() > 0)
                {
                    throw new Exception($"Usuário já existe: {usuario.NomeDeUsuario}");
                }
            }
        }

        public string GetPublicKey(string entity, string userName)
        {
            var cnpj = EncryptedText.Build(entity);
            var result = UsuarioAdministradorRepository.GetAll()
                .Where(p => p.Situacao == SituacaoDeUsuario.Ativo)
                .Where(i => i.NomeDeUsuario == userName)
                .Select(i => i.Certificado)
                .FirstOrDefault();
            return result.GetPlainText();
        }

        public long? GetUserId(string entity, string userName)
        {
            var cnpj = EncryptedText.Build(entity);
            var result = UsuarioAdministradorRepository.GetAll()
                .Where(p => p.Situacao == SituacaoDeUsuario.Ativo)
                .Where(i => i.NomeDeUsuario == userName)
                .Select(i => i.Id)
                .FirstOrDefault();
            return result;
        }
    }
}
