using System.Text;
using data;

namespace model
{
    public class Pais: ISearchableEntity
    {
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public virtual string NomeDaEntidade => "País";
        public virtual Genero GeneroDaEntidade => Genero.Masculino;
        public virtual string Nome { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Searchable { get; set; }

        public static string SearchableScope = "Pais";
        public string GetSearchableText()
        {
            var result = new StringBuilder();
            result.Append(Nome);
            return SearchableHelper.Build(result.ToString(), SearchableScope);
        }
    }
}
