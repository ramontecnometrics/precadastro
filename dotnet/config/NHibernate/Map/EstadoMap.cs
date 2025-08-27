using model;
using FluentNHibernate.Mapping;

namespace config.NHibernate.Map
{
    public class EstadoMap : ClassMap<Estado>
    {
        public EstadoMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Thumbprint).Length(36).Index("");
            Map(p => p.Nome).Length(50).Not.Nullable();
            Map(p => p.UF).Length(2).Not.Nullable();
            Map(p => p.CodigoIbge);
            References(p => p.Pais).Not.Nullable();
        }
    }
}