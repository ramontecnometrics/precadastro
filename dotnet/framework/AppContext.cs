using framework.Security;

namespace framework
{

    public interface IAppContext
    {
        IKeyProvider KeyProvider { get; }
        IAppUser User { get; }
        string PublicApiUrl { get; }
        void RegisterUserAction(UserAction action, string description);
        void RegisterUserAction(IAppUser user, UserAction action, string description);
    }

    public interface IAppUser
    {
        long? Id { get; }
        long? RepresentingId { get; }
        long? UnitId { get; }
    }
}
