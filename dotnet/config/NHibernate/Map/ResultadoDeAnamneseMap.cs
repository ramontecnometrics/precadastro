using FluentNHibernate.Mapping;
using model;

namespace config.NHibernate.Map
{
    public class ResultadoDeAnamneseMap : ClassMap<ResultadoDeAnamnese>
    {
        public ResultadoDeAnamneseMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Thumbprint).Length(36).Index("");
            Map(p => p.Data);
            References(p => p.Lead);
            HasMany(p => p.Itens)
                .Cascade.All();
        }
    }

    public class ResultadoDeFormularioDeAnamneseMap : ClassMap<ResultadoDeFormularioDeAnamnese>
    {
        public ResultadoDeFormularioDeAnamneseMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Thumbprint).Length(36).Index("");
            References(p => p.ResultadoDeAnamnese);
            References(p => p.ResultadoDeFormulario);
        }
    }
}
