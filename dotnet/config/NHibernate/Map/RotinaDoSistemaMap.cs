using model;
using FluentNHibernate.Mapping;

namespace config.NHibernate.Map
{
    public class RotinaDoSistemaMap : ClassMap<RotinaDoSistema>
    {
        public RotinaDoSistemaMap()
        {
            Id(p => p.Id).GeneratedBy.Assigned();
            Map(p => p.Thumbprint).Length(36).Index("");
            Map(p => p.Descricao).Not.Nullable();  
            Map(p => p.Searchable).Length(500).Index("");
        }
    }
}