using System.Collections.Generic;
using api.Dtos;
using data;
using framework.Extensions;
using model;

namespace api.Conversors
{
    public class EnderecoDePessoaConversor
    {
        private readonly Repository<EnderecoDePessoa> EnderecoDePessoaRepository;
        private readonly Repository<Cidade> CidadeRepository;
        private readonly Repository<Pais> PaisRepository;

        public EnderecoDePessoaConversor(Repository<EnderecoDePessoa> enderecoDePessoaRepository, 
        Repository<Cidade> cidadeRepository, Repository<Pais> paisRepository )
        {
            EnderecoDePessoaRepository = enderecoDePessoaRepository;
            CidadeRepository = cidadeRepository;
            PaisRepository = paisRepository;
        }

        public virtual void Fill<T, U>(T pessoa, U enderecos)
            where T : Pessoa, new()
            where U : IEnumerable<EnderecoDePessoaDto>
        {
            if (pessoa.Enderecos == null)
            {
                pessoa.Enderecos = new List<EnderecoDePessoa>();
            }

            if (enderecos == null)
            {
                return;
            }

            pessoa.Enderecos.Merge(
                enderecos,
                ((i, j) => i.Id == j.Id),
                (i =>
                {
                    pessoa.Enderecos.Remove(i);
                    EnderecoDePessoaRepository.Delete(i);
                }),
                ((i, j) =>
                {
                    i.Endereco.Logradouro = j.Endereco.Logradouro;
                    i.Endereco.Bairro = j.Endereco.Bairro;
                    i.Endereco.CEP = j.Endereco.CEP;
                    i.Endereco.Complemento = j.Endereco.Complemento;
                    i.Endereco.Numero = j.Endereco.Numero;
                    i.Endereco.Cidade = j.Endereco.Cidade != null ? CidadeRepository.Get(j.Endereco.Cidade.Id) : null;
                    i.Endereco.Pais = j.Endereco.Pais != null ? PaisRepository.Get(j.Endereco.Pais.Id) : null;
                    i.Endereco.Linha1 = j.Endereco.Linha1;
                    i.Endereco.Linha2 = j.Endereco.Linha2;
                    i.Endereco.Linha3 = j.Endereco.Linha3;
                }),
                (j =>
                {
                    var endereco = new EnderecoDePessoa()
                    {
                        Pessoa = pessoa,
                        Endereco = new Endereco()
                        {
                            Logradouro = j.Endereco.Logradouro,
                            Bairro = j.Endereco.Bairro,
                            CEP = j.Endereco.CEP,
                            Complemento = j.Endereco.Complemento,
                            Numero = j.Endereco.Numero,
                            Cidade = (j.Endereco != null && j.Endereco.Cidade != null) ?
                                    CidadeRepository.Get(j.Endereco.Cidade.Id, true) : null,
                            Linha1 = j.Endereco.Linha1,
                            Linha2 = j.Endereco.Linha2,
                            Linha3 = j.Endereco.Linha3,
                            Pais = (j.Endereco != null && j.Endereco.Cidade != null) ?
                                    CidadeRepository.Get(j.Endereco.Cidade.Id, true).Estado.Pais :
                                    (
                                        (j.Endereco != null && j.Endereco.Pais != null) ? PaisRepository.Get(j.Endereco.Pais.Id, true) : null
                                    )
                        },
                        Tipo = TipoDeEndereco.Comercial,
                    };
                    pessoa.Enderecos.Add(endereco);
                })
            );
        }
    }
}
