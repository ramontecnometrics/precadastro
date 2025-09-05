using framework.Extensions;
using model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace api.Dtos
{
    public class ResultadoDeFormularioDto
    {
        public GrupoDeResultadoDeFormularioDto[] Grupos { get; set; }

        public static ResultadoDeFormularioDto Build(IEnumerable<ResultadoDeFormulario> avaliacaoClinica)
        {
            if (avaliacaoClinica == null)
            {
                return null;
            }
            var gruposDto = new List<GrupoDeResultadoDeFormularioDto>();

            avaliacaoClinica.GroupBy(i => i.Campo.GrupoDeFormulario)
            .ForEach(i =>
            {
                var grupo = GrupoDeResultadoDeFormularioDto.Build(i.Key, i.ToArray());
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

        public static GrupoDeResultadoDeFormularioDto Build(GrupoDeFormulario item, ResultadoDeFormulario[] resultados)
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
        public string ValorFormatado { get; set; }
        public string Opcoes { get; set; }

        private static string FormatarValor(string tipo, string valor)
        {
            var result = valor;

            if (tipo == "opcoesmultiplas" && !valor.IsEmpty())
            {
                var valores = valor.Split("|");
                var valoresStrb = new StringBuilder();
                foreach (var v in valores)
                {
                    if (!v.IsEmpty())
                    {
                        valoresStrb.Append($"{v}, ");
                    }
                    result = valoresStrb.ToString();
                    if (!result.IsEmpty())
                    {
                        result = result.Remove(result.Length - 2);
                    }
                }
            }
            return result;
        }

        public static CampoDeResultadoDeFormularioDto Build(ResultadoDeFormulario item)
        {
            if (item == null)
            {
                return null;
            }

            var result = new CampoDeResultadoDeFormularioDto()
            {
                Id = item.Id,
                Tipo = item.Campo.Tipo,
                Opcoes = item.Campo.Opcoes,
                Titulo = item.Campo.Titulo,
                Valor = item.Valor,
                ValorFormatado = FormatarValor(item.Campo.Tipo, item.Valor)
            };
            return result;
        }
    }
}
