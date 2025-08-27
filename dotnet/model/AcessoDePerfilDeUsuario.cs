using data;
using System.Collections.Generic;

namespace model
{
    public class AcessoDePerfilDeUsuario: IEntity
    {
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public virtual RotinaDoSistema Rotina { get; set; }
        public virtual PerfilDeUsuario PerfilDeUsuario { get; set; }
        public virtual string NomeDaEntidade => "Acesso de perfil de usuário";
        public virtual Genero GeneroDaEntidade => Genero.Masculino;
    }

    internal class AcessoDePerfilDeUsuarioEqualityComparer : IEqualityComparer<AcessoDePerfilDeUsuario>
    {
        public bool Equals(AcessoDePerfilDeUsuario x, AcessoDePerfilDeUsuario y)
        {
            return (x.PerfilDeUsuario.Id == y.PerfilDeUsuario.Id) && (x.Rotina.Id == y.Rotina.Id);
        }

        public int GetHashCode(AcessoDePerfilDeUsuario obj)
        {
            return obj.Rotina.Id.GetHashCode() | obj.PerfilDeUsuario.Id.GetHashCode();
        }
    }

    internal class AcessoDePerfilDeUsuarioPorRotinaEqualityComparer : IEqualityComparer<AcessoDePerfilDeUsuario>
    {
        public bool Equals(AcessoDePerfilDeUsuario x, AcessoDePerfilDeUsuario y)
        {
            return (x.Rotina.Id == y.Rotina.Id);
        }

        public int GetHashCode(AcessoDePerfilDeUsuario obj)
        {
            return obj.Rotina.Id.GetHashCode();
        }
    }
    
}
