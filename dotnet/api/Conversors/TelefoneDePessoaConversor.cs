using System.Collections.Generic;
using api.Dtos;
using data;
using framework;
using framework.Extensions;
using model;

namespace api.Conversors
{
    public class TelefoneDePessoaConversor
    {
        private readonly Repository<TelefoneDePessoa> TelefoneDePessoaRepository;

        public TelefoneDePessoaConversor(Repository<TelefoneDePessoa> telefoneDePessoaRepository)
        {
            TelefoneDePessoaRepository = telefoneDePessoaRepository;
        }

        public virtual void Fill<T, U>(T pessoa, U telefones)
            where T : Pessoa, new()
            where U : IEnumerable<TelefoneDto>
        {
            if (pessoa.Telefones == null)
            {
                pessoa.Telefones = new List<TelefoneDePessoa>();
            }

            if (telefones == null)
            {
                return;
            }

            pessoa.Telefones.Merge(
                telefones,
                ((i, j) => (i.Id == j.Id)),
                (i =>
                {
                    pessoa.Telefones.Remove(i);
                    TelefoneDePessoaRepository.Delete(i);
                }),
                ((i, j) =>
                {
                    i.Numero = EncryptedText.Build(j.Numero);
                    i.Pais = j.Pais;
                    i.DDD = j.DDD;
                    i.TemWhatsApp = j.TemWhatsApp;
                }
                ),
                (j =>
                {
                    var telefone = new TelefoneDePessoa()
                    {
                        Pessoa = pessoa,
                        Numero = EncryptedText.Build(j.Numero),
                        Pais = j.Pais,
                        DDD = j.DDD,
                        TemWhatsApp = j.TemWhatsApp,
                    };
                    pessoa.Telefones.Add(telefone);
                })
            );
        }
    }
}
