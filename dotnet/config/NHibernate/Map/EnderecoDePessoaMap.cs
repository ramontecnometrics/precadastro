using model;
using data.Extensions;
using FluentNHibernate.Mapping;

namespace config.NHibernate.Map
{
    public class EnderecoDePessoaMap : ClassMap<EnderecoDePessoa>
    {
        public EnderecoDePessoaMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Thumbprint).Length(36).Index("");
            References(p => p.Pessoa).Not.Nullable();
            References(p => p.Endereco).Not.Nullable().Cascade.All();
            Map(p => p.Tipo)
                .CustomType<IntToTipoType<TipoDeEndereco>>()
                .Check(EnumExtensions.GetDatabaseCheckConstraint<EnderecoDePessoa, TipoDeEndereco>(p => p.Tipo));
        }
    }

}
