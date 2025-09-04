using data;
using System;
using System.Collections.Generic;

namespace model
{
    public class ResultadoDeAvaliacaoClinica: IEntity
    {
        public long Id { get; set; }
        public string Thumbprint { get; set; }
        public string NomeDaEntidade => "Resultado de avaliação";
        public Genero GeneroDaEntidade => Genero.Masculino;

        public Lead Lead { get; set; }
        public DateTime Data { get; set; }
        public IList<ResultadoDeFormularioDeAvaliacaoClinica> Itens { get; set; }
    }

    public class ResultadoDeFormularioDeAvaliacaoClinica : IEntity
    {
        public long Id { get; set; }
        public string Thumbprint { get; set; }
        public string NomeDaEntidade => "Resultado de avaliação clínica";
        public Genero GeneroDaEntidade => Genero.Masculino;

        public ResultadoDeAvaliacaoClinica ResultadoDeAvaliacaoClinica { get; set; }
        public ResultadoDeFormulario ResultadoDeFormulario { get; set; }

    }
}
