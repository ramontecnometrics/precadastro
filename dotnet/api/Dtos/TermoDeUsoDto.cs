using model;
using System;

namespace api.Dtos
{
    public class TermoDeUsoDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Termo { get; set; }
        public virtual DateTime DataDeCadastro { get; set; }

        public static TermoDeUsoDto Build(TermoDeUso termoDeUso)
        {
            if (termoDeUso == null)
            {
                return null;
            }

            var result = new TermoDeUsoDto()
            {
                Id = termoDeUso.Id,
                Nome = termoDeUso.Nome,
                Termo = termoDeUso.Termo,
                DataDeCadastro = termoDeUso.DataDeCadastro,
            };
            return result;
        }
    }
}
