using data;
using System.Text;

namespace model
{
    public class Cidade: ISearchableEntity
    {
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public virtual string NomeDaEntidade => "Cidade";
        public virtual Genero GeneroDaEntidade => Genero.Feminino;
        public virtual string Nome { get; set; }
        public virtual Estado Estado { get; set; }

        public static string SearchableScope = "Cidade";
        public virtual string Searchable { get; set; }
        public virtual string GetSearchableText()
        {
            var result = new StringBuilder();
            result.Append(Nome);
            return SearchableHelper.Build(result.ToString(), SearchableScope);
        }
    }
}
