using framework;
using model;

namespace api.Dtos
{
    public class EnderecoDto
    {
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string CEP { get; set; }
        public CidadeDto Cidade { get; set; }
        public string EnderecoCompleto { get; set; }
        public PaisDto Pais { get; set; }
        public string Linha1 { get; set; }
        public string Linha2 { get; set; }
        public string Linha3 { get; set; }

        public static EnderecoDto Build(Endereco item)
        {
            var result = default(EnderecoDto);
            if (item != null)
            {
                result = new EnderecoDto()
                {
                    Bairro = item.Bairro,
                    CEP = item.CEP,
                    Cidade = CidadeDto.Build(item.Cidade),
                    Complemento = item.Complemento,
                    Logradouro = item.Logradouro,
                    Numero = item.Numero,
                    Pais = PaisDto.Build(item.Pais),
                    Linha1 = item.Linha1,
                    Linha2 = item.Linha2,
                    Linha3 = item.Linha3,

                    EnderecoCompleto = item.EnderecoCompleto
                };
            }
            return result;
        }
    }

    public class EnderecoDePessoaDto
    {
        public long Id { get; set; }
        public long IdDaPessoa { get; set; }
        public Tipo<TipoDeEndereco> Tipo { get; set; }
        public EnderecoDto Endereco { get; set; }

        public static EnderecoDePessoaDto Build(EnderecoDePessoa item)
        {
            var result = default(EnderecoDePessoaDto);
            if (item != null)
            {
                result = new EnderecoDePessoaDto()
                {
                    Id = item.Id,
                    IdDaPessoa = item.Pessoa.Id,
                    Endereco = EnderecoDto.Build(item.Endereco),
                    Tipo = item.Tipo,
                };
            }
            return result;
        }
    }
}