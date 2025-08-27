using model;
using FluentNHibernate.Mapping;

namespace config.NHibernate.Map
{
    public class CodigoDeSegurancaParaRecuperacaoDeSenhaMap : ClassMap<CodigoDeSegurancaParaRecuperacaoDeSenha>
    {
        public CodigoDeSegurancaParaRecuperacaoDeSenhaMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Thumbprint).Length(36).Index("");
            Map(p => p.Codigo).Not.Nullable().Length(9);
            Map(p => p.HorarioDaEmissaoDoCodigo).Not.Nullable();
            References(p => p.Usuario).Not.Nullable();
        }
    }
}
