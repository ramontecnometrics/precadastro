using data;
using framework;
using System.Text;

namespace model
{
    public class Unidade: ISearchableEntity
    {
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public virtual string NomeDaEntidade => "Unidade";
        public virtual Genero GeneroDaEntidade => Genero.Feminino;
        public virtual string Nome { get; set; } 
        public virtual EncryptedText UnoSecretKey { get; set; }
        public virtual EncryptedText UnoAccessToken { get; set; }

        public static string SearchableScope = "Unidade";
        public virtual string Searchable { get; set; }
        public virtual string GetSearchableText()
        {
            var result = new StringBuilder();
            result.Append(Nome);
            return SearchableHelper.Build(result.ToString(), SearchableScope);
        }
    }
}
