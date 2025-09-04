using data.Extensions;
using FluentNHibernate.Mapping;
using model;
using framework;

namespace config.NHibernate.Map
{
    public class AuditoriaMap : ClassMap<Auditoria>
    {
        public AuditoriaMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Thumbprint).Length(36).Index("");
            Map(p => p.Data).Not.Nullable();
            Map(p => p.IdDoUsuario).Not.Nullable();
            Map(p => p.Descricao).CustomSqlType("text").Not.Nullable();            
            Map(p => p.Acao).CustomType<IntToTipoType<UserAction>>().Not.Nullable();
        }

        public class AuditoriaFastMap : ClassMap<AuditoriaFast>
        {
            public AuditoriaFastMap()
            {
                Id(p => p.Id);
                Map(p => p.Thumbprint);
                Map(p => p.Data);
                Map(p => p.IdDoUsuario);
                Map(p => p.NomeDoUsuario);
                Map(p => p.Descricao);
                Map(p => p.Acao).CustomType<IntToTipoType<UserAction>>();
                ReadOnly();
                SchemaAction.None();
                Subselect(@"        
select auditoria.id,
       auditoria.thumbprint,
       auditoria.data,
       auditoria.acao,
       auditoria.descricao,       
       usuario.NomeDeUsuario as nomedousuario,
       auditoria.iddousuario
  from auditoria
  join usuario on usuario.id = auditoria.iddousuario
    ");
            }
        }
    }
}