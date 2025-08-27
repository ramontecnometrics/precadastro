using model;
using data.Extensions;
using FluentNHibernate.Mapping;

namespace config.NHibernate.Map
{
    public class PessoaMap : ClassMap<Pessoa>
    {
        public PessoaMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Thumbprint).Length(36).Index("");
            Map(p => p.NomeCompleto).Length(1000);
            Map(p => p.Apelido).Length(1000);
            Map(p => p.RazaoSocial).Length(1000);
            Map(p => p.NomeFantasia).Length(1000);
            Map(p => p.DataDeCadastro).Not.Nullable();
            Map(p => p.Searchable).Length(500).Index("");

            Map(p => p.Sexo).CustomType<IntToTipoType<Sexo>>().Not.Nullable()
                .Check(EnumExtensions.GetDatabaseCheckConstraint<Pessoa, Sexo>(p => p.Sexo));
            HasMany(p => p.Enderecos)
                .Cascade.All()
                .Inverse();
            HasMany(p => p.Telefones)
                .ForeignKeyCascadeOnDelete()
                .Cascade.All()
                .Inverse();

            Map(p => p.TipoDePessoa).CustomType<IntToTipoType<TipoDePessoa>>()
                .Not.Nullable()
                .Check(EnumExtensions.GetDatabaseCheckConstraint<Pessoa, TipoDePessoa>(p => p.TipoDePessoa));
            Map(p => p.DocumentoDeIdentidade).Length(1000);
            Map(p => p.OrgaoExpedidorDoDocumentoDeIdentidade).Length(20);
            Map(p => p.Cpf).Length(1000);
            Map(p => p.Cnpj).Length(1000);
            Map(p => p.Email).Length(1000);
            References(p => p.Foto);
            References(p => p.Pais);
            Map(p => p.DataDeNascimento); 
        }
    }
}