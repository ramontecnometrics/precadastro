using data;
using framework;
using System.Collections.Generic;

namespace model
{
    public class EnderecoDePessoa: IEntity
    {
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public virtual string NomeDaEntidade => "Endereço";
        public virtual Genero GeneroDaEntidade => Genero.Masculino;
        public virtual Pessoa Pessoa { get; set; }
        public virtual Endereco Endereco { get; set; }
        public virtual Tipo<TipoDeEndereco> Tipo { get; set; }
    }

    internal class EnderecoDePessoaEqualityComparer : IEqualityComparer<EnderecoDePessoa>
    {
        public bool Equals(EnderecoDePessoa x, EnderecoDePessoa y)
        {
            return (x.Id == y.Id);
        }

        public int GetHashCode(EnderecoDePessoa obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}