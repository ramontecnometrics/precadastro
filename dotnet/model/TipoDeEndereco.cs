using framework;

namespace model
{
    public enum TipoDeEndereco
    {
        [Descricao("Não informado")]
        NaoInformado = 0,

        [Descricao("Residencial")]
        Residencial = 1,

        [Descricao("Comercial")]
        Comercial = 2,
    }
}