using System;
using model;

namespace api.Dtos
{
    public class AcessoDePerfilDeUsuarioDto
    {
        public long Id { get; set; }
        public RotinaDoSistemaDto Rotina { get; set; }
        public bool AcessoPermitido { get; set; }

        public static AcessoDePerfilDeUsuarioDto Build(RotinaDoSistema rotina, bool acessoPermitido)
        {
            if (rotina == null)
            {
                return null;
            }

            var result = new AcessoDePerfilDeUsuarioDto()
            {
                Rotina = RotinaDoSistemaDto.Build(rotina),
                AcessoPermitido = acessoPermitido
            };
            return result;
        }
    }
}
