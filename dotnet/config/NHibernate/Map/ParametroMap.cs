using model;
using FluentNHibernate.Mapping;

namespace config.NHibernate.Map
{
    public class ParametroDoSistemaMap : ClassMap<ParametroDoSistema>
    {
        public ParametroDoSistemaMap()
        {
            Id(i => i.Id).GeneratedBy.Identity();
            Map(p => p.Thumbprint).Length(36).Index("");
            Map(i => i.Grupo).Not.Nullable();
            Map(i => i.Ordem).Not.Nullable();
            Map(i => i.Nome).Not.Nullable().Length(150);
            Map(i => i.Descricao).Not.Nullable().Length(255);
            Map(i => i.Valor).Length(1000);
            Map(i => i.Protegido);
        }
    }
}