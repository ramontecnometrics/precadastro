using model;
using FluentNHibernate.Mapping;

namespace config.NHibernate.Map
{
    public class ArquivoMap : ClassMap<Arquivo>
    {
        public ArquivoMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Thumbprint).Length(36).Index("");
            Map(p => p.Nome).Not.Nullable().Length(255);
            Map(p => p.Tipo).Not.Nullable().Length(200);
            Map(p => p.Descricao).Length(255);
            Map(p => p.Temporario);
            Map(p => p.DataDeCriacao);
        }
    }
}
