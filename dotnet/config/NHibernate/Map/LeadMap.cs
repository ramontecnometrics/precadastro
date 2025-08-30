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
      
            References(x => x.Profissao);

            Map(x => x.Cnh);

            Map(x => x.EstadoCivil)
                .CustomType<IntToTipoType<EstadoCivil>>()                
                .Check(EnumExtensions.GetDatabaseCheckConstraint<Lead, EstadoCivil>(p => p.EstadoCivil));

            Map(x => x.Observacao).CustomSqlType("text");
            Map(x => x.AlertaDeSaude).CustomSqlType("text");
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
            Map(p => p.NomeCompleto).Length(200);
            Map(p => p.Email).Length(200);
            Map(p => p.DataDeCadastro);
            Map(x => x.Situacao).CustomType<IntToTipoType<SituacaoDeLead>>();
            References(p => p.Telefone);
            References(p => p.Celular);
            ReadOnly();
            SchemaAction.None();
            Subselect(
                @"
select lead.id,
    telefone.id as telefone_id,
    celular.id as celular_id,
    lead.searchable,
    pessoa.datadecadastro,
    pessoa.thumbprint,
    pessoa.nomecompleto,
    pessoa.cpf,
    pessoa.email,
    lead.situacao
  from lead
    join pessoa on lead.id = pessoa.id
    left join telefonedepessoa telefone on telefone.pessoa_id = pessoa.id and telefone.tipo = 2
    left join telefonedepessoa celular on celular.pessoa_id = pessoa.id and celular.tipo = 1
  where lead.situacao <> 3"
                );
        }
    }
}
