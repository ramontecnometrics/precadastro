using framework;
using model.Repositories;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using model;

namespace api
{
    public class WebApiUserValidator : IUserValidator
    {
        private readonly UsuarioRepository UsuarioRepository;
        private readonly ParametroDoSistemaRepository ParametrosRepository;

        public WebApiUserValidator(UsuarioRepository usuarioRepository,
            ParametroDoSistemaRepository parametrosRepository)
        {
            UsuarioRepository = usuarioRepository;
            ParametrosRepository = parametrosRepository;
        }

        public bool IsValid(string userId, string password, string role)
        {
            var item = UsuarioRepository.GetUsuarioByLoginESenha(userId, password, role );
            var result = (item != null) && (item.Situacao.Is(model.SituacaoDeUsuario.Ativo));
            return result;
        }

        public int GetExpireTimeInSeconds()
        {
            var horas = 240;
            var result = 60 * 60 * horas;
            var tempoDeSessaoEmMinutosSemAtividade =
                ParametrosRepository.GetInt("Autenticacao.TempoDeSessaoEmMinutosSemAtividade");
            if (tempoDeSessaoEmMinutosSemAtividade.HasValue)
            {
                result = tempoDeSessaoEmMinutosSemAtividade.Value * 60;
            }
            return result;
        }

        public bool CanExecuteOperation(User user, IOperation operation)
        {
            var result = false;
            if ((!operation.RequiresAuthentication) ||
                ((user.Id.HasValue) && (UsuarioRepository.TemAcesso(user.Id.Value,user.RepresentingId, operation.Id))))
            {
                result = true;
            };
            return result;
        }

        private bool TemAcesso(ModuleAccessControl i, string action, string module)
        {
            var result = false;
            if ((i.Action == action) || (i.Action == "*"))
            {
                if ((i.ModuleName == module))
                {
                    result = true;
                }
                else
                if (
                    (i.ModuleName.EndsWith('*')) &&
                    (module.StartsWith(i.ModuleName.Substring(0, i.ModuleName.Length - 1)))
                   )
                {
                    result = true;
                }
                else
                {
                    var regex = new Regex(i.ModuleName);
                    result = regex.IsMatch(module);
                }
            }
            return result;
        }

        private IEnumerable<ModuleAccessControl> FreeAccessModulos()
        {
            var result = new List<ModuleAccessControl>
            {
                new ModuleAccessControl() { ModuleName = "/login/token", Action = "POST" },
                new ModuleAccessControl() { ModuleName = "/file/*", Action = "GET" },
                new ModuleAccessControl() { ModuleName = "/defaulthub*", Action = "GET" },
            };
            return result.ToArray();
        }

        public void SetLastValidToken(string userId,  string origin, string tokenUid)
        {
            UsuarioRepository.SetLastValidToken(userId, origin, tokenUid);
        }

        public string GetLastValidTokenUID(string userId, string origin)
        {
            var result = UsuarioRepository.GetLastValidTokenUID(userId, origin);
            return result;
        }

        public string GetContractStatus(string userId, string origin)
        {
            var result = UsuarioRepository.GetContractStatus(userId, origin);
            return result;
        }

        public string GetPublicKey(string entity, string userName)
        {
            var result = UsuarioRepository.GetPublicKey(entity, userName);
            return result;
        }

        public long? GetUserId(string entity, string userName)
        {
            var result = UsuarioRepository.GetUserId(entity, userName);
            return result;
        }
    }

    internal class ModuleAccessControl
    {
        private string moduleName;
        private string action;
        public string ModuleName { get { return this.moduleName; } set { this.moduleName = value.ToLower(); } }
        public string Action { get { return this.action; } set { this.action = value.ToUpper(); } }
    }
}