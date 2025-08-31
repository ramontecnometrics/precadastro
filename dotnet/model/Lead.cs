using data;
using framework;
using System;
using System.Linq;
using System.Text;

namespace model
{
    public enum SituacaoDeLead
    {
        [Descricao("Não informado")]
        NaoDefinido = 0,
        [Descricao("Ativo")]
        Ativo = 1,
        [Descricao("Inativo")]
        Inativo = 2,
        [Descricao("Excluído")]
        Excluido = 3,
    }

    public enum EstadoCivil
    {
        [Descricao("Não informado")]
        NaoInformado = 0,
        [Descricao("Solteiro(a)")]
        Solteiro = 1,
        [Descricao("Casado(a)")]
        Casado = 2,
        [Descricao("Viúvo(a)")]
        Viuvo = 3,
        [Descricao("Divorciado(a)")]
        Divorciado = 4,
        [Descricao("Separado(a)")]
        Separado = 5,
        [Descricao("União civil")]
        UniaoCivil = 6,
    }

    public class Lead : Pessoa, ISearchableEntity
    {
        public override string NomeDaEntidade => "Lead";
        public override Genero GeneroDaEntidade => Genero.Masculino;
        public Tipo<SituacaoDeLead> Situacao { get; set; }
        public virtual TelefoneDePessoa Telefone
        {
            get
            {
                return Telefones != null ? Telefones.Where(i => i.Tipo == TipoDeTelefone.Residencial).FirstOrDefault() : null;
            }
        }
        public virtual TelefoneDePessoa Celular
        {
            get
            {
                return Telefones != null ? Telefones.Where(i => i.Tipo == TipoDeTelefone.Celular).FirstOrDefault() : null;
            }
        }
        public virtual EnderecoDePessoa Endereco
        {
            get
            {
                return Enderecos != null ? Enderecos.FirstOrDefault() : null;
            }
        }
        public virtual Profissao Profissao { get; set; }
        public EncryptedText Cnh { get; set; }
        public Tipo<EstadoCivil> EstadoCivil { get; set; }
        public string Observacao { get; set; }
        public string AlertaDeSaude { get; set; }
        public int? IdentificacaoNoUno { get; set; }
        public string TokenParaAvaliacaoClinica { get; set; }

        public static string SearchableScope = "Lead";
        public override string GetSearchableText()
        {
            var result = new StringBuilder();
            result.Append(Email.GetPlainText());
            result.Append(Telefone?.NumeroComDDD);
            result.Append(Celular?.NumeroComDDD);
            result.Append(NomeCompleto.GetPlainText());
            result.Append(Cpf.GetPlainText());
            result.Append(Cnh.GetPlainText());
            return SearchableHelper.Build(result.ToString(), SearchableScope);
        }
    }
    
    public class LeadFast : IEntity
    {
        public long Id { get; set; }
        public string Thumbprint { get; set; }
        public string NomeDaEntidade => "Lead";
        public Genero GeneroDaEntidade => Genero.Masculino;
        public EncryptedText Cpf { get; set; }
        public EncryptedText NomeCompleto { get; set; }
        public EncryptedText Email { get; set; }
        public TelefoneDePessoa Telefone { get; set; }
        public TelefoneDePessoa Celular { get; set; }
        public string Searchable { get; set; }
        public DateTime DataDeCadastro { get; set; }
        public Tipo<SituacaoDeLead> Situacao { get; set; }
    }

    public interface IResultadoDeFormulario
    {
        public long Id { get; set; }
        public CampoDeGrupoDeFormulario Campo { get; }
        public string Valor { get; }
    }

    public class ResultadoDeAvaliacaoClinica: IEntity, IResultadoDeFormulario
    {
        public long Id { get; set; }
        public string Thumbprint { get; set; }
        public string NomeDaEntidade => "Resultado de avaliação";
        public Genero GeneroDaEntidade => Genero.Masculino;

        public Lead Lead { get; set; }
        public CampoDeGrupoDeFormulario Campo { get; set; }
        public string Valor { get; set; }
    }
}
