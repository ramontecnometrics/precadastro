using model;
using FluentNHibernate.Mapping;

namespace config.NHibernate.Map
{
    public class PaisMap : ClassMap<Pais>
    {
        public PaisMap()
        {
            Id(p => p.Id).GeneratedBy.Assigned();
            Map(p => p.Thumbprint).Length(36).Index("");
            Map(p => p.Nome).Length(50).Not.Nullable().Unique();
            Map(p => p.Codigo).Length(3).Not.Nullable().Unique();
            Map(p => p.Searchable).Length(500).Not.Nullable();
        }
    }
}