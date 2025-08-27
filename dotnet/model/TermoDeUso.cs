using data;
using System;
using System.Text;

namespace model
{

    public class TermoDeUso : ISearchableEntity
    {  
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public virtual string NomeDaEntidade => "Termo de uso";
        public virtual Genero GeneroDaEntidade => Genero.Masculino;
        public virtual string Nome { get; set; }
        public virtual string Termo { get; set; }
        public virtual DateTime DataDeCadastro { get; set; }

        public virtual string Searchable { get; set; }
        public static string SearchableScope = "Pais";
        public virtual string GetSearchableText()
        {
            var result = new StringBuilder();
            result.Append(Nome);
            return SearchableHelper.Build(result.ToString(), SearchableScope);
        }
    }
}
