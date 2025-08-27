using data;
using System.Collections.Generic;

namespace model
{
    public class Estado: IEntity
    {
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public virtual string NomeDaEntidade => "Estado";
        public virtual Genero GeneroDaEntidade => Genero.Masculino;
        public virtual string UF { get; set; }
        public virtual string Nome {get;set;}
        public virtual Pais Pais { get; set; }
        public virtual int CodigoIbge { get; set; }
    }
}
