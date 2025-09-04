using FluentNHibernate.Mapping;
using model;

namespace config.NHibernate.Map
{
    public class ResultadoDeAvaliacaoClinicaMap : ClassMap<ResultadoDeAvaliacaoClinica>
    {
        public ResultadoDeAvaliacaoClinicaMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Thumbprint).Length(36).Index("");
            Map(p => p.Data);
            References(p => p.Lead);
            HasMany(p => p.Itens)
                .Cascade.All();
        }
    }

    public class ResultadoDeFormularioDeAvaliacaoClinicaMap : ClassMap<ResultadoDeFormularioDeAvaliacaoClinica>
    {
        public ResultadoDeFormularioDeAvaliacaoClinicaMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Thumbprint).Length(36).Index("");            
            References(p => p.ResultadoDeAvaliacaoClinica);
            References(p => p.ResultadoDeFormulario);            
        }
    }    
}
