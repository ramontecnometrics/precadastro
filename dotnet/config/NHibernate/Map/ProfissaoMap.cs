using data.Extensions;
using FluentNHibernate.Mapping;
using model;

namespace config.NHibernate.Map
{
    public class ProfissaoMap : ClassMap<Profissao>
    {
        public ProfissaoMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Thumbprint).Length(36).Index("");
            Map(x => x.Nome).Length(100).Not.Nullable();
            Map(x => x.Situacao)
                .CustomType<IntToTipoType<SituacaoDeProfissao>>()
                .Not.Nullable()
                .Check(EnumExtensions.GetDatabaseCheckConstraint<Profissao, SituacaoDeProfissao>(p => p.Situacao));
            Map(x => x.Searchable).Length(500).Index("");
        }
    }
}
