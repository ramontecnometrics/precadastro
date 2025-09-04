using api.Dtos;
using model;
using System;
using System.Linq;

namespace api.Controllers
{
    public class ResultadoDeAvaliacaoClinicaDto
    {
        public long Id { get; set; }
        public DateTime Data { get; set; }
        public ResultadoDeFormularioDto Resultado { get; set; }

        public static ResultadoDeAvaliacaoClinicaDto Build(ResultadoDeAvaliacaoClinica item)
        {
            if (item == null)
            {
                return null;
            }

            var result = new ResultadoDeAvaliacaoClinicaDto()
            {
                Id = item.Id,
                Data = item.Data,
                Resultado = ResultadoDeFormularioDto.Build(item.Itens.Select(i => i.ResultadoDeFormulario).ToArray())
            };
            return result;
        }
    }

}
