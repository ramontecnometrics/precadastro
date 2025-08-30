using model;
using data.Extensions;
using FluentNHibernate.Mapping;

namespace config.NHibernate.Map
{
    public class TelefoneMap : ClassMap<TelefoneDePessoa>
    {
        public TelefoneMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Thumbprint).Length(36).Index("");
            Map(p => p.Pais).Not.Nullable().Length(3);
            Map(p => p.DDD).Not.Nullable().Length(3);
            Map(p => p.Numero).Not.Nullable().Length(30);
            Map(p => p.TemWhatsApp);
            Map(p => p.Tipo).CustomType<IntToTipoType<TipoDeTelefone>>().Not.Nullable()
                .Check(EnumExtensions.GetDatabaseCheckConstraint<TelefoneDePessoa, TipoDeTelefone>(p => p.Tipo));
            References(p => p.Pessoa);
        }
    }
}