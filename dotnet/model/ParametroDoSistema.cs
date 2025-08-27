using data;
using framework;
using framework.Validators;
using System.Text;

namespace model
{
    public class ParametroDoSistema : IEntity
    {
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public virtual string NomeDaEntidade => "Parâmetro do Sistema";
        public virtual Genero GeneroDaEntidade => Genero.Masculino;
        [RequiredValidation("Informe o nome do parâmetro.")]
        public virtual int Grupo { get; set; }
        public virtual int Ordem { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Descricao { get; set; }
        public virtual EncryptedText Valor { get; set; }
        public virtual bool Protegido { get; set; }
 
        public virtual int TermoDeUsoVigente  { get; set; }
    }

}
