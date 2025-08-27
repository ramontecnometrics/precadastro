using data;
using framework;
using System.Collections.Generic;
using framework.Validators;
using System.Text;
using System.Linq;
using framework.Extensions;

namespace model
{
    public static class Roles {
        public const string ADM = "ADM";
    }

    public class UsuarioLogado : IEntity
    {
        public long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public Genero GeneroDaEntidade => Genero.Masculino;
        public string NomeDaEntidade => "Login";

        public Tipo<TipoDePerfilDeUsuario> Perfil { get; set; }
        public UsuarioLogado Representando { get; set; }

        public UsuarioLogado Usuario
        {
            get
            {
                return this.Representando != null ? this.Representando : this;
            }
        }
    }

    public enum SituacaoDeUsuario
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

    public enum OrigemDeAcessoDeUsuario
    {
        [Descricao("Não definido")]
        NaoDefinido = 0,

        [Descricao("Web")]
        Web = 1,

        [Descricao("Mobile")]
        Mobile = 2,

        [Descricao("Desktop")]
        Desktop = 3,
    }

    public abstract class Usuario : ISearchableEntity
    {
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public virtual string NomeDaEntidade => "Usuário";
        public virtual Genero GeneroDaEntidade => Genero.Masculino;
        [RequiredValidation("Informe o nome da pessoa.")]
        public virtual EncryptedText Nome { get; set; }
        [RequiredValidation("Informe o nome de usuário para acesso ao sistema")]
        public virtual string NomeDeUsuario { get; set; }
        public virtual EncryptedText Senha { get; set; }
        public virtual Arquivo Foto { get; set; }
        public virtual IList<PerfilDoUsuario> Perfis { get; set; }
        [RequiredValidation("Informe a situação do usuário.")]
        public virtual Tipo<SituacaoDeUsuario> Situacao { get; set; }
        public virtual EncryptedText PushToken { get; set; }
        public virtual string Searchable { get; set; }
        public virtual string LastValidTokenUidForWeb { get; set; }
        public virtual string LastValidTokenUidForDesktop { get; set; }
        public virtual string LastValidTokenUidForMobile { get; set; }
        public virtual EncryptedText Email { get; set; }       
        public virtual bool AceitouTermosDeUso { get; set; }
        public virtual EncryptedText Certificado { get; set; }

        public static string SearchableScope = "Usuario";
        public virtual string GetSearchableText()
        {
            var result = new StringBuilder();
            result.Append(Nome.GetPlainText());
            result.Append(NomeDeUsuario);
            result.Append(Email.GetPlainText());
            return SearchableHelper.Build(result.ToString(), SearchableScope);
        }

        public virtual bool IsMaster()
        {
            var result = Perfis.Where(i => i.Perfil.TipoDePerfil == TipoDePerfilDeUsuario.Administrativo &&
                i.Perfil.Nome == "MASTER").Any();
            return result;
        }

        public virtual bool IsAdministrativo()
        {
            var result = Perfis.Where(i => i.Perfil.TipoDePerfil == TipoDePerfilDeUsuario.Administrativo).Any();
            return result;
        }

        public IEnumerable<AcessoDePerfilDeUsuario> GetAcessos()
        {
            var acessos = new List<AcessoDePerfilDeUsuario>();
            this.Perfis.ForEach(i =>
            {
                if (i.Perfil.Acessos != null)
                {
                    acessos.AddRange(i.Perfil.Acessos);
                }
            });
            var comparer = new AcessoDePerfilDeUsuarioPorRotinaEqualityComparer();
            var result = acessos.Distinct(comparer);
            return result;
        }
    }

    public class PerfilDoUsuario : IEntity
    {
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public virtual string NomeDaEntidade => "Perfil do Usuário";
        public virtual Genero GeneroDaEntidade => Genero.Masculino;
        [RequiredValidation("Informe o usuário.")]
        public Usuario Usuario { get; set; }
        [RequiredValidation("Informe o perfil de usuário.")]
        public PerfilDeUsuario Perfil { get; set; }
    }
}
