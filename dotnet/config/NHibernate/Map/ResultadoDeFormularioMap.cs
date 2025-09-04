using FluentNHibernate.Mapping;
using model;

namespace config.NHibernate.Map
{
    public class ResultadoDeFormularioMap : ClassMap<ResultadoDeFormulario>
    {
        public ResultadoDeFormularioMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Thumbprint).Length(36).Index("");            
            References(p => p.Campo);
            Map(p => p.Valor).Length(500);
        }
    }
}
