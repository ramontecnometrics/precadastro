using data;
using framework;
using System.Collections.Generic;
using System.Linq;
using framework.Validators;
using System;
using System.Text;
using framework.Extensions;

namespace model
{
    public enum TipoDePessoa
    {
        [Descricao("Não informado")]
        NaoInformado = 0,

        [Descricao("Física")]
        PessoaFisica = 1,

        [Descricao("Jurídica")]
        PessoaJuridica = 2,
    }

    public enum Sexo
    {
        [Descricao("Não informado")]
        NaoInformado = 0,

        [Descricao("Masculino")]
        Masculino = 1,

        [Descricao("Feminino")]
        Feminino = 2,
    }

    public abstract class Pessoa : ISearchableEntity
    {
        public virtual long Id { get; set; }
        public virtual string Thumbprint { get; set; }

        public virtual string Searchable { get; set; }
        [RequiredValidation("Informe a data de cadastro.")]
        public virtual DateTime DataDeCadastro { get; set; }        
        public virtual EncryptedText Apelido { get; set; }
        public virtual EncryptedText Cpf { get; set; }
        public virtual EncryptedText Cnpj { get; set; }
        public virtual EncryptedText DocumentoDeIdentidade { get; set; }
        public virtual string OrgaoExpedidorDoDocumentoDeIdentidade { get; set; }
        public virtual IList<EnderecoDePessoa> Enderecos { get; set; }
        public virtual IList<TelefoneDePessoa> Telefones { get; set; }
        public virtual EncryptedText Email { get; set; }
        public virtual Tipo<Sexo> Sexo { get; set; }
        public virtual Tipo<TipoDePessoa> TipoDePessoa { get; set; }
        public virtual Arquivo Foto { get; set; }
        public virtual DateTime? DataDeNascimento { get; set; } 
        public virtual Pais Pais { get; set; }

        [RequiredValidation("Informe o nome da pessoa.", "Nome", GroupValidationType.AtLeastOneValid)]
        public virtual EncryptedText NomeCompleto { get; set; }

        [RequiredValidation("Informe a razão social.", "Nome", GroupValidationType.AtLeastOneValid)]
        public virtual EncryptedText RazaoSocial { get; set; }

        public virtual EncryptedText NomeFantasia { get; set; }

        public virtual string IdadePorExtenso { get { return DataDeNascimento.ToIdadePorExtenso(); } }
        public virtual int Idade { get { return DataDeNascimento.ToIdade(); } }

        public abstract Genero GeneroDaEntidade { get; }
        public abstract string NomeDaEntidade { get; }

        public Pessoa()
        {
            this.Sexo = 0;
            this.TipoDePessoa = 0;
        }

        public virtual TelefoneDePessoa GetTelefonePrincipal()
        {
            var result = default(TelefoneDePessoa);
            if (Telefones != null)
            {
                var telefone = Telefones.FirstOrDefault();
                if (telefone != null)
                {
                    result = telefone;
                }
            }
            return result;
        }

        public virtual EnderecoDePessoa GetEnderecoPrincipal()
        {
            var result = default(EnderecoDePessoa);
            if (Enderecos != null)
            {
                var endereco = Enderecos.FirstOrDefault();
                if (endereco != null)
                {
                    result = endereco;
                }
            }
            return result;
        }

        public abstract string GetSearchableText();
    }

    public static class DateTimeExtensions
    {
        public static string ToIdadePorExtenso(this DateTime? data)
        {
            var result = default(string);
            if (data.HasValue)
            {
                DateTime DataNascimento = data.Value;
                DateTime DataAtual = DateTime.Now;
                if (DataNascimento > DataAtual)
                {
                    var exception = new Exception("A data de nascimento não pode ser superior a data atual.");                    
                    throw exception;
                }
                int Anos = new DateTime(DateTime.Now.Subtract(DataNascimento).Ticks).Year - 1;
                DateTime AnosTranscorridos = DataNascimento.AddYears(Anos);
                int Meses = 0;
                for (int i = 1; i <= 12; i++)
                {
                    if (AnosTranscorridos.AddMonths(i) == DataAtual)
                    {
                        Meses = i;
                        break;
                    }
                    else if (AnosTranscorridos.AddMonths(i) >= DataAtual)
                    {
                        Meses = i - 1;
                        break;
                    }
                }
                int Dias = DataAtual.Subtract(AnosTranscorridos.AddMonths(Meses)).Days;
                int Horas = DataAtual.Subtract(AnosTranscorridos).Hours;
                int Minutos = DataAtual.Subtract(AnosTranscorridos).Minutes;
                int Segundos = DataAtual.Subtract(AnosTranscorridos).Seconds;

                result = $"{Anos} anos {Meses} meses {Dias} dias";
            }
            return result;
        }

        public static int ToIdade(this DateTime? data)
        {
            var result = default(int);
            if (data.HasValue)
            {
                DateTime DataNascimento = data.Value;
                DateTime DataAtual = DateTime.Now;
                result = new DateTime(DateTime.Now.Subtract(DataNascimento).Ticks).Year - 1;
            }
            return result;
        }
    }
}
