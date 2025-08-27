using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace api.Log
{
    internal enum Categoria
    {
        [Description("Geral")]
        Geral = 1,
    }

    internal enum SubCategoria
    {
        [Description("Entrada")]
        Entrada = 1,

        [Description("Saída")]
        Saida = 2,

        [Description("Erro")]
        Erro = 3,
    }

    internal enum Modulo
    {
        [Description("WebApi")]
        WebApi = 1,
    }
}
