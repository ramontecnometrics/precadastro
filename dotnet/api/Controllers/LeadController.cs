using System.Linq;
using framework;
using model;
using api.Dtos;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System;
using model.Repositories;
using framework.Extensions;
using data;
using System.Text;

namespace api.Controllers
{
    [Route("lead/[action]")]
    [Route("lead")]
    public class LeadController :
           EntityFastController<
                 Lead,
                 LeadDto,
                 LeadFast,
                 LeadFastDto,
                 LeadGetParams,
                 LeadPostParams,
                 LeadPutParams>
    {
        protected override UserAction UserActionForInsert => UserAction.InseriuLead;
        protected override UserAction UserActionForUpdate => UserAction.AlterouLead;
        protected override UserAction UserActionForDelete => UserAction.ExcluiuLead;

        private readonly LeadRepository LeadRepository;
        private readonly Repository<Arquivo> ArquivoRepository;        
        private readonly Repository<Profissao> ProfissaoRepository;
        private readonly Repository<TelefoneDePessoa> TelefoneDePessoaRepository;
        private readonly Repository<EnderecoDePessoa> EnderecoDePessoaRepository;

        public LeadController(
            LeadRepository repository,
            Repository<LeadFast> fastRepository,
            Repository<Arquivo> arquivoRepository,
            Repository<Profissao> profissaoRepository,
            Repository<TelefoneDePessoa> telefoneDePessoaRepository,
            Repository<EnderecoDePessoa> enderecoDePessoaRepository,
            IAppContext appContext) :
            base(repository, fastRepository, appContext)
        {
            LeadRepository = repository;
            ArquivoRepository = arquivoRepository;            
            ProfissaoRepository = profissaoRepository;
            TelefoneDePessoaRepository = telefoneDePessoaRepository;
            EnderecoDePessoaRepository = enderecoDePessoaRepository;
        }

        protected override IQueryable<Lead> Get(LeadGetParams getParams)
        {
            var result = Repository.GetAll()
                .Where(i => i.Situacao != SituacaoDeLead.Excluido);
            if (getParams.Id.HasValue)
            {
                result = result.Where(i => i.Id == getParams.Id.Value);
            }
            if (!string.IsNullOrEmpty(getParams.Searchable))
            {
                result = result.Where(i => i.Searchable.Contains(getParams.Searchable));
            }
            if (!string.IsNullOrEmpty(getParams.Cpf))
            {
                result = result.Where(i => i.Cpf.GetPlainText().Contains(getParams.Cpf));
            }
            if (!string.IsNullOrEmpty(getParams.Email))
            {
                result = result.Where(i => i.Email.GetPlainText().Contains(getParams.Email));
            }
            if (getParams.Situacao.HasValue)
            {
                result = result.Where(i => i.Situacao == getParams.Situacao.Value);
            }

            return result;
        }

        protected override IQueryable<LeadFast> FastGet(LeadGetParams getParams)
        {
            var result = FastRepository.GetAll();
            if (getParams.Id.HasValue)
            {
                result = result.Where(i => i.Id == getParams.Id.Value);
            }
            if (!string.IsNullOrEmpty(getParams.Searchable))
            {
                result = result.Where(i => i.Searchable.Contains(getParams.Searchable));
            }
            return result;
        }

        protected override LeadDto Convert(Lead entity)
        {
            var result = LeadDto.Build(entity);
            return result;
        }

        protected override LeadFastDto Convert(LeadFast entity)
        {
            var result = LeadFastDto.Build(entity);
            return result;
        }

        protected override Lead Convert(LeadPostParams insertRequest)
        {
            var lead = new Lead()
            {
                NomeCompleto = EncryptedText.Build(insertRequest.NomeCompleto),
                Cpf = EncryptedText.Build(insertRequest.Cpf),
                Cnpj = EncryptedText.Build(insertRequest.Cnpj),
                Email = EncryptedText.Build(insertRequest.Email),
                Cnh = EncryptedText.Build(insertRequest.Cnh),
                Observacao = insertRequest.Observacao,
                AlertaDeSaude = insertRequest.AlertaDeSaude,
                EstadoCivil = insertRequest.EstadoCivil,
                Situacao = SituacaoDeLead.Ativo,
                DataDeCadastro = DateTime.Now,
                Telefones = new List<TelefoneDePessoa>(),
                Enderecos = new List<EnderecoDePessoa>()
            };

            if (insertRequest.Profissao != null)
            {
                lead.Profissao = ProfissaoRepository.Get(insertRequest.Profissao.Id, true);
            }

            if (insertRequest.Telefone != null)
            {
                var telefone = new TelefoneDePessoa()
                {
                    Numero = EncryptedText.Build(insertRequest.Telefone.Numero),
                    DDD = insertRequest.Telefone.DDD,
                    Tipo = insertRequest.Telefone.Tipo,
                    Pessoa = lead
                };
                lead.Telefones.Add(telefone);
                lead.Telefone = telefone;
            }

            if (insertRequest.Endereco != null)
            {
                var endereco = new EnderecoDePessoa()
                {
                    Endereco = new Endereco()
                    {
                        Logradouro = insertRequest.Endereco.Endereco?.Logradouro,
                        Numero = insertRequest.Endereco.Endereco?.Numero,
                        Complemento = insertRequest.Endereco.Endereco?.Complemento,
                        Bairro = insertRequest.Endereco.Endereco?.Bairro,
                        CEP = insertRequest.Endereco.Endereco?.CEP,
                        Cidade = insertRequest.Endereco.Endereco?.Cidade != null ? 
                            Repository<Cidade>.Get(insertRequest.Endereco.Endereco.Cidade.Id, true) : null
                    },
                    Tipo = insertRequest.Endereco.Tipo,
                    Pessoa = lead
                };
                lead.Enderecos.Add(endereco);
                lead.Endereco = endereco;
            }

            if (insertRequest.Foto != null)
            {
                lead.Foto = ArquivoRepository.Get(insertRequest.Foto.Id, true);
            }

            return lead;
        }

        protected override Lead Convert(LeadPutParams updateRequest, Lead oldLead)
        {
            oldLead.NomeCompleto = EncryptedText.Build(updateRequest.NomeCompleto);
            oldLead.Cpf = EncryptedText.Build(updateRequest.Cpf);
            oldLead.Cnpj = EncryptedText.Build(updateRequest.Cnpj);
            oldLead.Email = EncryptedText.Build(updateRequest.Email);
            oldLead.Cnh = EncryptedText.Build(updateRequest.Cnh);
            oldLead.Observacao = updateRequest.Observacao;
            oldLead.AlertaDeSaude = updateRequest.AlertaDeSaude;
            oldLead.EstadoCivil = updateRequest.EstadoCivil;
            oldLead.Situacao = updateRequest.Situacao;
            oldLead.Foto = updateRequest.Foto != null 
                ? ArquivoRepository.Get(updateRequest.Foto.Id, true) 
                : null;

            if (updateRequest.Profissao != null)
            {
                oldLead.Profissao = ProfissaoRepository.Get(updateRequest.Profissao.Id, true);
            }
            else
            {
                oldLead.Profissao = null;
            }

            // Atualizar telefone
            if (updateRequest.Telefone != null)
            {
                if (oldLead.Telefone == null)
                {
                    oldLead.Telefone = new TelefoneDePessoa()
                    {
                        Pessoa = oldLead
                    };
                    oldLead.Telefones.Add(oldLead.Telefone);
                }
                oldLead.Telefone.Numero = EncryptedText.Build(updateRequest.Telefone.Numero);
                oldLead.Telefone.DDD = updateRequest.Telefone.DDD;
                oldLead.Telefone.Tipo = updateRequest.Telefone.Tipo;
            }

            // Atualizar endereço
            if (updateRequest.Endereco != null)
            {
                if (oldLead.Endereco == null)
                {
                    oldLead.Endereco = new EnderecoDePessoa()
                    {
                        Pessoa = oldLead
                    };
                    oldLead.Enderecos.Add(oldLead.Endereco);
                }
                if (oldLead.Endereco.Endereco == null)
                {
                    oldLead.Endereco.Endereco = new Endereco();
                }
                oldLead.Endereco.Endereco.Logradouro = updateRequest.Endereco.Endereco?.Logradouro;
                oldLead.Endereco.Endereco.Numero = updateRequest.Endereco.Endereco?.Numero;
                oldLead.Endereco.Endereco.Complemento = updateRequest.Endereco.Endereco?.Complemento;
                oldLead.Endereco.Endereco.Bairro = updateRequest.Endereco.Endereco?.Bairro;
                oldLead.Endereco.Endereco.CEP = updateRequest.Endereco.Endereco?.CEP;
                oldLead.Endereco.Endereco.Cidade = updateRequest.Endereco.Endereco?.Cidade != null ? 
                    Repository<Cidade>.Get(updateRequest.Endereco.Endereco.Cidade.Id, true) : null;
                oldLead.Endereco.Tipo = updateRequest.Endereco.Tipo;
            }

            return oldLead;
        }
    }

    public class LeadGetParams : IId
    {
        public long? Id { get; set; }
        public string Searchable { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public Tipo<SituacaoDeLead>? Situacao { get; set; }
    }

    public class LeadPostParams : IPostParams
    {
        public string NomeCompleto { get; set; }
        public string Cpf { get; set; }
        public string Cnpj { get; set; }
        public string Email { get; set; }
        public string Cnh { get; set; }
        public string Observacao { get; set; }
        public string AlertaDeSaude { get; set; }
        public Tipo<EstadoCivil> EstadoCivil { get; set; }
        public TelefoneDePessoaDto Telefone { get; set; }
        public EnderecoDePessoaDto Endereco { get; set; }
        public ProfissaoDto Profissao { get; set; }
        public ArquivoDto Foto { get; set; }
    }

    public class LeadPutParams : IPutParams
    {
        public long? Id { get; set; }
        public string NomeCompleto { get; set; }
        public string Cpf { get; set; }
        public string Cnpj { get; set; }
        public string Email { get; set; }
        public string Cnh { get; set; }
        public string Observacao { get; set; }
        public string AlertaDeSaude { get; set; }
        public Tipo<EstadoCivil> EstadoCivil { get; set; }
        public Tipo<SituacaoDeLead> Situacao { get; set; }
        public TelefoneDePessoaDto Telefone { get; set; }
        public EnderecoDePessoaDto Endereco { get; set; }
        public ProfissaoDto Profissao { get; set; }
        public ArquivoDto Foto { get; set; }
    }
}
