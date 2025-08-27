using System;
using model;

namespace api.Dtos
{
    public class PerfilDoUsuarioDto
    {
         public long Id { get; private set; }
        public PerfilDeUsuarioDto Perfil { get; set; }       

        public static PerfilDoUsuarioDto Build(PerfilDoUsuario value)
        {
            if (value == null)
            {
                return null;
            }
            var result = new PerfilDoUsuarioDto()
            {
                Id = value.Id,
                Perfil = PerfilDeUsuarioDto.Build(value.Perfil)
            };
            return result;
        }
    }
}
