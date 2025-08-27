using api.Controllers;
using framework;
using framework.Security;
using model.Repositories;

namespace api
{
    public class WebApiAppContext : IAppContext
    {
        public IKeyProvider KeyProvider { get; set; }
        public IAppUser User { get; set; }
        public string PublicApiUrl { get; set; }
        public AuditoriaRepository AuditoriaRepository { get; set; }

        public virtual void RegisterUserAction(UserAction action, string description)
        {
            if (User.Id.HasValue)
            {
                AuditoriaRepository.RegistrarAcao(                    
                    User.Id.Value,
                    action,
                    description
                );
            }
        }

        public void RegisterUserAction(IAppUser user, UserAction action, string description)
        {
            if (user != null)
            {
                AuditoriaRepository.RegistrarAcao(                    
                    user.Id.Value,
                    action,
                    description
                );
            }
        }
    }

    public class WebApiUser : IAppUser
    {
        public long? Id { get; set; }
        public long? RepresentingId { get; set; }
        public long? UnitId { get; set; }

        private readonly model.UsuarioLogado _usuarioLogado;

        public WebApiUser(model.UsuarioLogado usuarioLogado)
        {
            Id = usuarioLogado?.Id;
            RepresentingId = usuarioLogado?.Representando?.Id;
            _usuarioLogado = usuarioLogado;
        }

        public model.UsuarioLogado GetUsuarioLogado()
        {
            return _usuarioLogado;
        }

    }

    public static class IAppContextExtensions
    {
        public static model.UsuarioLogado Usuario(this IAppContext appContext)
        {
            var result = default(model.UsuarioLogado);
            var webApiAppContext = appContext as WebApiAppContext;
            if (webApiAppContext != null)
            {
                result = ((WebApiUser)webApiAppContext.User).GetUsuarioLogado();
                if (result != null && result.Representando != null)
                {
                    result = result.Representando;
                }
            }
            return result;
        }

        public static model.UsuarioLogado UsuarioLogado(this IAppContext appContext)
        {
            var result = default(model.UsuarioLogado);
            var webApiAppContext = appContext as WebApiAppContext;
            if (webApiAppContext != null)
            {
                result = ((WebApiUser)webApiAppContext.User).GetUsuarioLogado();
            }
            return result;
        }
    }
}
