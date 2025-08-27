using data;
using System;

namespace model
{
    public class Arquivo : IEntity
    {
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public virtual string NomeDaEntidade => "Arquivo";
        public virtual Genero GeneroDaEntidade => Genero.Masculino;
        public virtual string Tipo { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Descricao { get; set; }
        public virtual bool Temporario { get; set; }
        public virtual DateTime DataDeCriacao { get; set; }
    }
}
