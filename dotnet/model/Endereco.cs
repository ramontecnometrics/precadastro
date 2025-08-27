using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace model
{
    public class Endereco
    {
        public virtual long Id { get; set; }
        public virtual string Logradouro { get; set; }
        public virtual string Numero { get; set; }
        public virtual string Complemento { get; set; }
        public virtual string Bairro { get; set; }
        public virtual string CEP { get; set; }
        public virtual Cidade Cidade { get; set; }
        public virtual string UF { get { return (Cidade?.Estado.UF); } }
        public virtual string EnderecoCompleto { get { return this.ToEnderecoCompleto(); } }
        public virtual Pais Pais { get; set; }
        public virtual string Linha1 { get; set; }
        public virtual string Linha2 { get; set; }
        public virtual string Linha3 { get; set; }
    }

    public static class EnderecoExtensions
    {
        public static string ToEnderecoCompleto(this Endereco endereco)
        {
            var result = string.Empty;
            if (endereco !=  null)
            {
                if (endereco.Cidade != null && endereco.Cidade.Estado.Pais.Codigo == "+55")
                {
                    result = string.Join(", ", new string[]
                    {
                        endereco.Logradouro,
                        endereco.Numero,
                        endereco.Complemento,
                        endereco.Bairro,
                        $"CEP {endereco.CEP}", 
                        $"{endereco.Cidade.Nome} / {endereco.Cidade.Estado.UF}"
                    });
                    result = result.Replace(", ,", ",").Trim();
                } else {
                    result = string.Join(", ", new string[]
                    {
                        endereco.Linha1,
                        endereco.Linha2,
                        endereco.Linha3,                       
                    });
                }
            }
            return result;
        }
    }
}
