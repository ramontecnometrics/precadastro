using data;
using framework;
using framework.Validators;
using System.Text;

namespace model
{
    public enum SituacaoDeProfissao
    {
        [Descricao("Não definida")]
        NaoDefinido = 0,
        [Descricao("Ativo")]
        Ativo = 1,
        [Descricao("Inativo")]
        Inativo = 2
    }

    public class Profissao : ISearchableEntity
    {
        public Genero GeneroDaEntidade => Genero.Feminino;
        public string NomeDaEntidade => "Profissão";

        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        [RequiredValidation("Informe o nome da profissão.")]
        public virtual string Nome { get; set; }
        public virtual Tipo<SituacaoDeProfissao> Situacao { get; set; }

        public virtual string Searchable { get; set; }
        public static string SearchableScope = "Profissao";
        public virtual string GetSearchableText()
        {
            var result = new StringBuilder();
            result.Append(Nome);            
            return SearchableHelper.Build(result.ToString(), SearchableScope);
        }         
    }
}
