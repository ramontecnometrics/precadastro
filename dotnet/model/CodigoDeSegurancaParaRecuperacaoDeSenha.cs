using data;
using System;
using System.Collections.Generic;
using System.Text;

namespace model
{
    public class CodigoDeSegurancaParaRecuperacaoDeSenha : CodigoDeSeguranca, IEntity
    {
        public Usuario Usuario { get; set; }

        public bool CodigoDeSegurancaValido(CodigoDeSegurancaParaRecuperacaoDeSenha codigoDeSeguranca)
        {            
            if (codigoDeSeguranca.HorarioDaEmissaoDoCodigo < DateTime.Now.AddMinutes(-5))
            {
                return false;
            }
            else
            {
                return true;
            }

        }
    }

}
