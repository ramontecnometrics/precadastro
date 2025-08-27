using System.Collections.Generic;
using data;
using framework;

namespace model
{
    public class UsuarioAdministrador : Usuario
    {
    }

    public class UsuarioAdministradorFast : IEntity
    {
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public virtual string NomeDaEntidade => "Pessoa";
        public virtual Genero GeneroDaEntidade => Genero.Masculino;
        public virtual string Searchable { get; set; }
        public virtual string NomeDeUsuario { get; set; }
        public virtual long IdDoUsuario { get; set; } 
        public virtual EncryptedText Email { get; set; }
        public virtual EncryptedText Nome { get; set; }
    }
}
