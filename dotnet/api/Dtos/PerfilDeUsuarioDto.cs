using framework;
using model;

namespace api.Dtos
{
    public class PerfilDeUsuarioDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public Tipo<SituacaoDePerfilDeUsuario> Situacao { get; set; }
        public Tipo<TipoDePerfilDeUsuario> TipoDePerfil { get; set; }
        public AcessoDePerfilDeUsuarioDto[] Acessos { get; set; }

        public static PerfilDeUsuarioDto Build(PerfilDeUsuario value)
        {
            if (value == null)
            {
                return null;
            }
            var result = new PerfilDeUsuarioDto()
            {
                Id = value.Id, 
                Nome = value.Nome,
                TipoDePerfil = value.TipoDePerfil
            };
            return result;
        }
    }

    public class PerfilDeUsuarioFastDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public Tipo<SituacaoDePerfilDeUsuario> Situacao { get; set; }
        public Tipo<TipoDePerfilDeUsuario> TipoDePerfil { get; set; }
    }

}
