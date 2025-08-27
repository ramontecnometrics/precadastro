using data.Extensions;
using FluentNHibernate.Mapping;
using model;

namespace config.NHibernate.Map
{
    public class LeadMap : SubclassMap<Lead>
    {
        public LeadMap()
        {
            Map(x => x.Searchable).Length(500).Index("");
            Map(x => x.Thumbprint).Length(36).Index("");

            Map(x => x.Situacao)
                .CustomType<IntToTipoType<SituacaoDeLead>>()
                .Not.Nullable()
                .Check(EnumExtensions.GetDatabaseCheckConstraint<Lead, SituacaoDeLead>(p => p.Situacao));

            References(x => x.Telefone);
            References(x => x.Endereco);
            References(x => x.Profissao);

            Map(x => x.Cnh);

            Map(x => x.EstadoCivil)
                .CustomType<IntToTipoType<EstadoCivil>>()
                .Not.Nullable()
                .Check(EnumExtensions.GetDatabaseCheckConstraint<Lead, EstadoCivil>(p => p.EstadoCivil));

            Map(x => x.Observacao).CustomType("text");
            Map(x => x.AlertaDeSaude).CustomType("text");
        }
    }

    public class LeadFastMap : ClassMap<LeadFast>
    {
        public LeadFastMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Thumbprint).Length(36).Index("");            
            Map(p => p.Searchable).Length(500).Index("");
            Map(p => p.Cpf).Length(20);
            Map(p => p.Nome).Length(200);
            Map(p => p.Email).Length(200);
            ReadOnly();
            SchemaAction.None();
            //Se for usar uma view tem que passar o script aqui para poder criar a view automaticamente no setup.
            Views.RegisterViewScript(@"drop view leadfast", true);
            Views.RegisterViewScript(@"
create or replace view leadfast as 
select a.id,
       b.thumbprint,
       b.nomecompleto, 
       a.searchable, 
       b.nomefantasia, 
       b.cpf,
       lead.telefone,
       a.email
  from lead a
  join pessoa b
    on a.id = b.id
 where a.situacao <> 3");
        }
    }
}
