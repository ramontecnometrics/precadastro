using FluentNHibernate.Mapping;
using model;

namespace config.NHibernate.Map
{
    public class UsuarioAdministradorMap : SubclassMap<UsuarioAdministrador>
    {
        public UsuarioAdministradorMap()
        {                         
        }
    }

    public class UsuarioAdministradorFastMap : ClassMap<UsuarioAdministradorFast>
    {
        public UsuarioAdministradorFastMap()
        {
            Id(p => p.Id);
            Map(p => p.Thumbprint).Length(36).Index("");
            Map(p => p.Nome); 
            Map(p => p.Searchable);
            Map(p => p.NomeDeUsuario);
            Map(p => p.IdDoUsuario);
            Map(p => p.Email);
            ReadOnly();
            SchemaAction.None();
            Views.RegisterViewScript(@"drop view usuarioadministradorfast", true);
            Views.RegisterViewScript(@"        
create or replace view usuarioadministradorfast as 
select usuario.id,
       usuario.thumbprint,      
       usuario.email,
       usuario.nomedeusuario,
       usuario.id as iddousuario,
       usuario.nome
  from usuarioadministrador 
  join usuario
    on usuario.id = usuarioadministrador.id");
        }
    }
}