using model;
using FluentNHibernate.Mapping;

namespace config.NHibernate.Map
{
    public class UnidadeMap : ClassMap<Unidade>
    {
        public UnidadeMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Thumbprint).Length(36).Index("");
            Map(p => p.Nome).Not.Nullable().Length(100);
            Map(p => p.UnoSecretKey).Length(1000);
            Map(p => p.UnoAccessToken).Length(1000);
            Map(p => p.Searchable).Not.Nullable().Length(500);
        }
    }
}
