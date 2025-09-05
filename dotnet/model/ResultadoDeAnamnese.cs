using data;
using System;
using System.Collections.Generic;

namespace model
{
    public class ResultadoDeAnamnese : IEntity
    {
        public long Id { get; set; }
        public string Thumbprint { get; set; }
        public string NomeDaEntidade => "Resultado de anamnese";
        public Genero GeneroDaEntidade => Genero.Masculino;

        public Lead Lead { get; set; }
        public DateTime Data { get; set; }
        public IList<ResultadoDeFormularioDeAnamnese> Itens { get; set; }
    }

    public class ResultadoDeFormularioDeAnamnese : IEntity
    {
        public long Id { get; set; }
        public string Thumbprint { get; set; }
        public string NomeDaEntidade => "Resultado de anamnese";
        public Genero GeneroDaEntidade => Genero.Masculino;

        public ResultadoDeAnamnese ResultadoDeAnamnese { get; set; }
        public ResultadoDeFormulario ResultadoDeFormulario { get; set; }

    }
}
