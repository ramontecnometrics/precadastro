using model;
using FluentNHibernate.Mapping;

namespace config.NHibernate
{
    public class TermoDeUsoMap : ClassMap<TermoDeUso>
    {
        public TermoDeUsoMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Searchable).Length(500).Not.Nullable().Index("");
            Map(p => p.Thumbprint).Length(36).Index("");
            Map(p => p.Nome).Not.Nullable().Length(100);
            Map(p => p.Termo).Not.Nullable().CustomSqlType("text");
            Map(p => p.DataDeCadastro).Not.Nullable();
        }
    }
}
