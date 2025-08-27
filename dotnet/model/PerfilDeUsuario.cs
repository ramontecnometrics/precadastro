using data;
using framework;
using framework.Validators;
using System.Collections.Generic;
using System.Text;

namespace model
{
    public enum SituacaoDePerfilDeUsuario
    {
        [Descricao("Não definido")]
        NaoDefinido = 0,

        [Descricao("Ativo")]
        Ativo = 1,

        [Descricao("Inativo")]
        Inativo = 2,

        [Descricao("Excluído")]
        Excluido = 3,        
    }

    public enum TipoDePerfilDeUsuario
    {
        [Descricao("Não definido")]
        NaoDefinido = 0,

        [Descricao("Adminstrativo")]
        Administrativo = 1,
    }

    public class PerfilDeUsuario : ISearchableEntity
    {
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public virtual string NomeDaEntidade => "Perfil de usuário";
        public virtual Genero GeneroDaEntidade => Genero.Masculino;
        [RequiredValidation("Informe o perfil de usuário.")]
        public virtual string Nome { get; set; }
        [RequiredValidation("Informe a situação do perfil de usuário.")]
        public virtual Tipo<SituacaoDePerfilDeUsuario> Situacao { get; set; }
        public virtual Tipo<TipoDePerfilDeUsuario> TipoDePerfil { get; set; }
        public virtual IList<AcessoDePerfilDeUsuario> Acessos { get; set; }    

        public virtual string Searchable { get; set; }
        public static string SearchableScope = "PerfilDeUsuario";
        public virtual string GetSearchableText()
        {
            var result = new StringBuilder();
            result.Append(Nome);
            return SearchableHelper.Build(result.ToString(), SearchableScope);
        }
    }

    public class PerfilDeUsuarioFast : IEntity
    {
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public virtual string NomeDaEntidade => "Perfil de usuário";
        public virtual Genero GeneroDaEntidade => Genero.Masculino;        
        public virtual string Nome { get; set; }                    
        public virtual string Searchable { get; set; }                    
        public virtual Tipo<SituacaoDePerfilDeUsuario> Situacao { get; set; }
        public virtual Tipo<TipoDePerfilDeUsuario> TipoDePerfil { get; set; }

    }
}
