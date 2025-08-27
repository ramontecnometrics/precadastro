using data;
using System.Collections.Generic;
using System.Text;
using framework;

namespace model
{    
    public abstract class RotinaDoSistemaBase : ISearchableEntity
    {
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public virtual string NomeDaEntidade => "Rotina do sistema";
        public virtual Genero GeneroDaEntidade => Genero.Feminino;
        public virtual string Descricao { get; set; }

        public virtual string Searchable { get; set; }
        public static string SearchableScope = "RotinaDoSistema";
        public virtual string GetSearchableText()
        {
            var result = new StringBuilder();
            result.Append(Descricao);
            return SearchableHelper.Build(result.ToString(), SearchableScope);
        }
    }

    public class RotinaDoSistema : RotinaDoSistemaBase, IOperation
    {
        public virtual string Description { get { return this.Descricao; } }
        public virtual bool RequiresAuthentication { get { return true; } }
        public bool OpenOperation { get; set;}
        public bool RestrictedOperation { get; set; }
    }
}