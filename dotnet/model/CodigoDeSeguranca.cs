using data;
using System;
using System.Collections.Generic;
using System.Text;

namespace model
{
    public class CodigoDeSeguranca
    {
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public virtual string NomeDaEntidade => "Código de segurança";
        public virtual Genero GeneroDaEntidade => Genero.Masculino;
        public virtual string  Codigo { get; set; }
        public virtual DateTime HorarioDaEmissaoDoCodigo { get; set; }
    }
}
