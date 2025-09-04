using model;
using data.Extensions;
using FluentNHibernate.Mapping;

namespace config.NHibernate.Map
{
    public class PerfilDeUsuarioMap : ClassMap<PerfilDeUsuario>
    {
        public PerfilDeUsuarioMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Thumbprint).Length(36).Index("");
            Map(p => p.Nome).Not.Nullable().Length(100);
            Map(p => p.Situacao).CustomType<IntToTipoType<SituacaoDePerfilDeUsuario>>().Not.Nullable()
                .Check(EnumExtensions.GetDatabaseCheckConstraint<PerfilDeUsuario, SituacaoDePerfilDeUsuario>(p => p.Situacao));
            Map(p => p.TipoDePerfil).CustomType<IntToTipoType<TipoDePerfilDeUsuario>>().Not.Nullable()
                .Check(EnumExtensions.GetDatabaseCheckConstraint<PerfilDeUsuario, TipoDePerfilDeUsuario>(p => p.TipoDePerfil));
            HasMany(p => p.Acessos)
                .Cascade.All()
                .Inverse();
            Map(p => p.Searchable).Length(500).Index("");
        }
    }

    public class PerfilDeUsuarioFastMap : ClassMap<PerfilDeUsuarioFast>
    {
        public PerfilDeUsuarioFastMap()
        {
            Id(p => p.Id);
            Map(p => p.Nome);
            Map(p => p.Searchable);
            Map(p => p.Situacao).CustomType<IntToTipoType<SituacaoDePerfilDeUsuario>>();
            Map(p => p.TipoDePerfil).CustomType<IntToTipoType<TipoDePerfilDeUsuario>>();
            ReadOnly();
            SchemaAction.None();
            Subselect(@"
select id, 
       thumbprint, 
       nome, 
       searchable,        
       situacao,        
       tipodeperfil 
  from perfildeusuario");
        }
    }
}