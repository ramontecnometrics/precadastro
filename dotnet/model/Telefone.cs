using data;
using framework;
using System.Collections.Generic;

namespace model
{
    public enum TipoDeTelefone
    {
        NaoDefinido = 0,
        Celular = 1,
        Residencial = 2,
        Comercial = 3,
    }

    public class Telefone
    {
        public virtual string Pais { get; set; }
        public virtual int DDD { get; set; }
        public virtual EncryptedText Numero { get; set; }
        public virtual bool TemWhatsApp { get; set; }
        public virtual Tipo<TipoDeTelefone> Tipo { get; set; }

        public virtual string NumeroComDDD
        {
            get
            {
                return this.DDD > 0 && !string.IsNullOrWhiteSpace(Numero.GetPlainText()) ? string.Format(
                    "({0}){1}",
                    this.DDD.ToString("00"),
                    this.GetNumeroComDigito()) : null;
            }
        }

        public virtual string NumeroComDDI
        {
            get
            {
                return this.DDD > 0 && !string.IsNullOrWhiteSpace(Numero.GetPlainText()) ? string.Format(
                    "({0}){1}",
                    this.DDD.ToString("00"),
                    this.GetNumeroComDigito()) : null;
            }
        }

        public Telefone()
        {
            Pais = "+55";
            Tipo = TipoDeTelefone.NaoDefinido;
        }

        private string GetNumeroComDigito()
        {
            if (this.Numero.GetPlainText().Length == 8)
            {
                return $"{this.Numero.GetPlainText().Substring(0, 4)}-{this.Numero.GetPlainText().Substring(4, 4)}";
            }
            else
            if (this.Numero.GetPlainText().Length == 9)
            {
                return $"{this.Numero.GetPlainText().Substring(0, 5)}-{this.Numero.GetPlainText().Substring(5, 4)}";
            }
            else
            {
                return this.Numero.GetPlainText();
            }
        }
    }

    public class TelefoneDePessoa: Telefone, IEntity
    {
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }
        public virtual string NomeDaEntidade => "Telefone";
        public virtual Genero GeneroDaEntidade => Genero.Masculino;
        public virtual Pessoa Pessoa { get; set; }       
    }  

}
