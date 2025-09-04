using data;

namespace model
{
    public class ResultadoDeFormulario : IEntity 
    {
        public long Id { get; set; }
        public string Thumbprint { get; set; }
        public string NomeDaEntidade => "Resultado de preenchimento de formulário";
        public Genero GeneroDaEntidade => Genero.Masculino;
        
        public CampoDeGrupoDeFormulario Campo { get; set; }
        public string Valor { get; set; }
    }
}
