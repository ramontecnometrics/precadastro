using framework.Extensions;
using model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace api.Dtos
{
    public class ResultadoDeFormularioDto
    {
        public GrupoDeResultadoDeFormularioDto[] Grupos { get; set; }

        public static ResultadoDeFormularioDto Build(IEnumerable<ResultadoDeAvaliacaoClinica> avaliacaoClinica)
        {
            if (avaliacaoClinica == null)
            {
                return null;
            }
            var gruposDto = new List<GrupoDeResultadoDeFormularioDto>();

            avaliacaoClinica.GroupBy(i => i.Campo.GrupoDeFormulario)
            .ForEach(i => {
                var grupo = GrupoDeResultadoDeFormularioDto.Build(i.Key, i.Select(k => k).ToArray());
                gruposDto.Add(grupo);                
            });

            var result = new ResultadoDeFormularioDto()
            {
                Grupos = gruposDto.ToArray()
            };
            
            return result;
        }
    }

    public class GrupoDeResultadoDeFormularioDto
    {
        public long Id { get; set; }
        public string Titulo { get; set; }
        public CampoDeResultadoDeFormularioDto[] Campos { get; set; }

        public static GrupoDeResultadoDeFormularioDto Build(GrupoDeFormulario item, IResultadoDeFormulario[] resultados)
        {
            if (item == null)
            {
                return null;
            }

            var result = new GrupoDeResultadoDeFormularioDto()
            {
                Id = item.Id,
                Titulo = item.Titulo,
                Campos = resultados.Select(i => CampoDeResultadoDeFormularioDto.Build(i)).ToArray()
            };

            return result;
        }
    }

    public class CampoDeResultadoDeFormularioDto
    {
        public long Id { get; set; }
        public string Titulo { get; set; }
        public string Tipo { get; set; }
        public string Valor { get; set; }

        public static CampoDeResultadoDeFormularioDto Build(IResultadoDeFormulario item)
        {
            if (item == null)
            {
                return null;
            }

            var result = new CampoDeResultadoDeFormularioDto()
            {
                Id = item.Id,
                Tipo = item.Campo.Tipo,
                Titulo = item.Campo.Titulo,
                Valor = item.Valor
            };
            return result;
        }
    }
}
