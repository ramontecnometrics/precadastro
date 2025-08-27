using FluentNHibernate.Mapping;
using model;

namespace config.NHibernate.Map
{
    public class AcessoDePerfilDeUsuarioMap : ClassMap<AcessoDePerfilDeUsuario>
    {
        public AcessoDePerfilDeUsuarioMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Thumbprint).Length(36).Index("");
            References(p => p.Rotina).Not.Nullable();
            References(p => p.PerfilDeUsuario);
        }
    }
}
