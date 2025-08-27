using data;
using framework;
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
        NaoInformado = 0,
        Solteiro = 1,
        Casado = 2,
        Viuvo = 3,
        Divorciado = 4,
        Separado = 5,
        UniaoCivil = 6,
    }

    public class Lead : Pessoa, ISearchableEntity
    {
        public override string NomeDaEntidade => "Lead";
        public override Genero GeneroDaEntidade => Genero.Masculino;
        public virtual Tipo<SituacaoDeLead> Situacao { get; set; }
        public TelefoneDePessoa Telefone { get; set; }
        public EnderecoDePessoa Endereco { get; set; }
        public Profissao Profissao { get; set; }
        public EncryptedText Cnh { get; set; }
        public virtual Tipo<EstadoCivil> EstadoCivil { get; set; }
        public string Observacao { get; set; }
        public string AlertaDeSaude { get; set; }

        public static string SearchableScope = "Lead";
        public override string Searchable { get; set; }
        public override string GetSearchableText()
        {
            var result = new StringBuilder();
            result.Append(Email.GetPlainText());
            result.Append(Telefone != null ? Telefone.NumeroComDDD : null);
            result.Append(NomeCompleto.GetPlainText());
            result.Append(Cpf.GetPlainText());
            result.Append(Cnh.GetPlainText());
            return SearchableHelper.Build(result.ToString(), SearchableScope);
        }
    }

    public class LeadFast : IEntity
    {
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public virtual string NomeDaEntidade => "Lead";
        public virtual Genero GeneroDaEntidade => Genero.Masculino;
        public virtual EncryptedText Cpf { get; set; }
        public virtual EncryptedText Nome { get; set; }
        public virtual EncryptedText Email { get; set; }
        public virtual EncryptedText Telefone { get; set; }
        public virtual string Searchable { get; set; }
    }
}
