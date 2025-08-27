using model;
using FluentNHibernate.Mapping;

namespace config.NHibernate.Map
{
    public class CidadeMap : ClassMap<Cidade>
    {
        public CidadeMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Thumbprint).Length(36).Index("");
            Map(p => p.Nome).Not.Nullable().Length(100);
            References(p => p.Estado).Not.Nullable();
            Map(p => p.Searchable).Not.Nullable().Length(500);
        }
    }
}
