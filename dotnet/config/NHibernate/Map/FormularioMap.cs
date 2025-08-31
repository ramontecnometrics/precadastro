using FluentNHibernate.Mapping;
using model;

namespace config.NHibernate.Map
{
    public class FormularioMap : ClassMap<Formulario>
    {
        public FormularioMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Searchable).Length(500).Index("");
            Map(x => x.Thumbprint).Length(36).Index("");
            Map(x => x.Nome).Length(200);
            Map(x => x.Descricao).Length(1000);
            HasMany(x => x.Grupos)
                .Inverse()
                .Cascade.All()
                .OrderBy("ordem");
        }
    }

    public class GrupoDeFormularioMap : ClassMap<GrupoDeFormulario>
    {
        public GrupoDeFormularioMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Thumbprint).Length(36).Index("");
            Map(x => x.Titulo).Length(200);
            Map(x => x.Ordem);
            References(x => x.Formulario)
                .Not.Nullable();
            HasMany(x => x.Campos)
                .Inverse()
                .Cascade.All()
                .OrderBy("ordem");
        }
    }

    public class CampoDeGrupoDeFormularioMap : ClassMap<CampoDeGrupoDeFormulario>
    {
        public CampoDeGrupoDeFormularioMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Thumbprint).Length(36).Index("");
            Map(x => x.Titulo).Length(200).Not.Nullable();
            Map(x => x.Tipo).Length(100).Not.Nullable();
            Map(x => x.Obrigatorio);
            Map(x => x.Ordem);
            References(x => x.GrupoDeFormulario)
              .Not.Nullable();
        }
    }

    public class FormularioFastMap : ClassMap<FormularioFast>
    {
        public FormularioFastMap()
        {
            Id(x => x.Id);
            Map(x => x.Searchable);
            Map(x => x.Thumbprint);
            Map(x => x.Nome);            
            ReadOnly();
            SchemaAction.None();
            Subselect(
                @"
select id,       
       searchable,       
       thumbprint,
       nome
from formulario");
        }
    }
}
