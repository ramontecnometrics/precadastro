using model;
using config.NHibernate;
using data.Extensions;
using FluentNHibernate.Mapping;

namespace config.NHibernate.Map
{
    public class UsuarioMap : ClassMap<Usuario>
    {
        public UsuarioMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Thumbprint).Length(36).Index("");
            Map(p => p.Nome).Length(1000).Not.Nullable();
            Map(p => p.Certificado).CustomSqlType("text");
            Map(p => p.NomeDeUsuario).Length(100).Not.Nullable();
            Map(p => p.Senha).Length(100);
            Map(p => p.PushToken).Length(100);
            HasMany(p => p.Perfis)
                .Inverse()
                .Cascade.All();
            Map(p => p.Situacao).CustomType<IntToTipoType<SituacaoDeUsuario>>().Not.Nullable()
                .Check(EnumExtensions.GetDatabaseCheckConstraint<Usuario, SituacaoDeUsuario>(p => p.Situacao));
            Map(p => p.Searchable).Length(500).Index("");
            Map(p => p.Email).Length(500);
            Map(p => p.AceitouTermosDeUso);
            Map(p => p.LastValidTokenUidForDesktop).Length(36);
            Map(p => p.LastValidTokenUidForMobile).Length(36);
            Map(p => p.LastValidTokenUidForWeb).Length(36);
            References(p => p.Foto);
        }
    }

    public class PerfilDoUsuarioMap : ClassMap<PerfilDoUsuario>
    {
        public PerfilDoUsuarioMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Thumbprint).Length(36).Index("");
            References(p => p.Usuario);
            References(p => p.Perfil);
        }
    }    

    public class UsuarioLogadoMap : ClassMap<UsuarioLogado>
    {
        public UsuarioLogadoMap()
        {
            Id(p => p.Id);            
            Map(p => p.Perfil).CustomType<IntToTipoType<TipoDePerfilDeUsuario>>();
            Map(p => p.Thumbprint).Length(36).Index("");
            ReadOnly();
            SchemaAction.None();
            Subselect(@"
select id, 1 perfil, cast (null as varchar(36)) thumbprint from usuarioadministrador");
        }
    }
}