using model;
using FluentNHibernate.Mapping;

namespace config.NHibernate.Map
{
    public class EnderecoMap : ClassMap<Endereco>
    {
        public EnderecoMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.Logradouro) ;
            Map(p => p.Numero).Length(10) ;
            Map(p => p.Complemento).Length(30);
            Map(p => p.Bairro);
            References(p => p.Cidade);
            Map(p => p.CEP);
            References(p => p.Pais);
            Map(p => p.Linha1);
            Map(p => p.Linha2);
            Map(p => p.Linha3);
        }
    }
}