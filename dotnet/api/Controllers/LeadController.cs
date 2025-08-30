using api.Dtos;
using data;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentNHibernate.Conventions;
using framework;
using framework.Extensions;
using Microsoft.AspNetCore.Mvc;
using model;
using model.Repositories;
using Remotion.Linq.Clauses.ResultOperators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using RouteAttribute = Microsoft.AspNetCore.Components.RouteAttribute;

namespace api.Controllers
{
    [Route("lead/[action]")]
    [Route("lead")]
    [ApiController]
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
        private readonly Repository<Cidade> CidadeRepository;

        public LeadController(
            LeadRepository repository,
            Repository<LeadFast> fastRepository,
            Repository<Arquivo> arquivoRepository,
            Repository<Profissao> profissaoRepository,
            Repository<TelefoneDePessoa> telefoneDePessoaRepository,
            Repository<EnderecoDePessoa> enderecoDePessoaRepository,
            IAppContext appContext,
            Repository<Cidade> cidadeRepository) :
            base(repository, fastRepository, appContext)
        {
            LeadRepository = repository;
            ArquivoRepository = arquivoRepository;
            ProfissaoRepository = profissaoRepository;
            TelefoneDePessoaRepository = telefoneDePessoaRepository;
            EnderecoDePessoaRepository = enderecoDePessoaRepository;
            CidadeRepository = cidadeRepository;
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
                result = result.Where(i => i.Searchable.Contains(SearchableHelper.Build(getParams.Searchable, Lead.SearchableScope)));
            }
            if (!string.IsNullOrEmpty(getParams.Cpf))
            {
                result = result.Where(i => i.Cpf == EncryptedText.Build(getParams.Cpf));
            }
            if (!string.IsNullOrEmpty(getParams.Celular))
            {
                var celularParts = getParams.Celular.Trim().Split(" ");
                var pais = "+" + celularParts[0].Trim();
                var ddd = int.Parse(celularParts[1].Replace("(", "").Replace(")", "").Trim());
                var celular = celularParts[2].Replace("-", "");
                result = result.Where(i =>
                    i.Telefones
                    .Where(i => i.Tipo == TipoDeTelefone.Celular)
                    .Where(i =>
                        i.Numero == EncryptedText.Build(celular) &&
                        i.DDD == ddd &&
                        i.Pais == pais
                    ).Any()
                );
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
                result = result.Where(i => i.Searchable.Contains(SearchableHelper.Build(getParams.Searchable, Lead.SearchableScope)));
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
                Sexo = insertRequest.Sexo,
                Telefones = new List<TelefoneDePessoa>(),
                Enderecos = new List<EnderecoDePessoa>(),
                DataDeNascimento = insertRequest.DataDeNascimento,
                DocumentoDeIdentidade = EncryptedText.Build(insertRequest.DocumentoDeIdentidade),
            };

            if (insertRequest.Profissao != null)
            {
                lead.Profissao = insertRequest.Profissao != null ? ProfissaoRepository.Get(insertRequest.Profissao, true) : null;
            }

            if (insertRequest.Telefone != null)
            {
                var telefone = new TelefoneDePessoa()
                {
                    Pessoa = lead,
                    Numero = EncryptedText.Build(insertRequest.Telefone.Numero),
                    DDD = insertRequest.Telefone.DDD,
                    Tipo = TipoDeTelefone.Residencial,
                };
                lead.Telefones.Add(telefone);
            }

            if (insertRequest.Celular != null)
            {
                var celular = new TelefoneDePessoa()
                {
                    Pessoa = lead,
                    Numero = EncryptedText.Build(insertRequest.Celular.Numero),
                    DDD = insertRequest.Celular.DDD,
                    Tipo = TipoDeTelefone.Celular,
                };
                lead.Telefones.Add(celular);
            }

            if (insertRequest.Endereco != null)
            {
                var endereco = new EnderecoDePessoa()
                {
                    Pessoa = lead,
                    Endereco = new Endereco()
                    {
                        Logradouro = insertRequest.Endereco.Endereco?.Logradouro,
                        Numero = insertRequest.Endereco.Endereco?.Numero,
                        Complemento = insertRequest.Endereco.Endereco?.Complemento,
                        Bairro = insertRequest.Endereco.Endereco?.Bairro,
                        CEP = insertRequest.Endereco.Endereco?.CEP,
                        Cidade = insertRequest.Endereco.Endereco?.Cidade != null ?
                            CidadeRepository.Get(insertRequest.Endereco.Endereco.Cidade.Id, true) : null
                    },
                    Tipo = TipoDeEndereco.Residencial,
                };
                lead.Enderecos.Add(endereco);
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
            oldLead.DocumentoDeIdentidade = EncryptedText.Build(updateRequest.DocumentoDeIdentidade);
            oldLead.Observacao = updateRequest.Observacao;
            oldLead.AlertaDeSaude = updateRequest.AlertaDeSaude;
            oldLead.EstadoCivil = updateRequest.EstadoCivil;
            oldLead.Situacao = updateRequest.Situacao;
            oldLead.DataDeNascimento = updateRequest.DataDeNascimento;
            oldLead.Sexo = updateRequest.Sexo;
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
                    oldLead.Telefones.Add(new TelefoneDePessoa()
                    {
                        Tipo = TipoDeTelefone.Residencial
                    });
                }
                oldLead.Telefone.Numero = EncryptedText.Build(updateRequest.Telefone.Numero);
                oldLead.Telefone.DDD = updateRequest.Telefone.DDD;
                oldLead.Telefone.Tipo = updateRequest.Telefone.Tipo;
            }

            if (updateRequest.Celular != null)
            {
                if (oldLead.Celular == null)
                {
                    oldLead.Telefones.Add(new TelefoneDePessoa()
                    {
                        Tipo = TipoDeTelefone.Celular
                    });
                }
                oldLead.Celular.Numero = EncryptedText.Build(updateRequest.Celular.Numero);
                oldLead.Celular.DDD = updateRequest.Celular.DDD;
                oldLead.Celular.Tipo = updateRequest.Celular.Tipo;
            }

            // Atualizar endereço
            if (updateRequest.Endereco != null)
            {
                if (oldLead.Endereco == null)
                {
                    oldLead.Enderecos.Add(new EnderecoDePessoa() { });
                }
                if (oldLead.Endereco.Endereco == null)
                {
                    oldLead.Endereco.Endereco = new Endereco();
                }
                oldLead.Endereco.Pessoa = oldLead;
                oldLead.Endereco.Endereco.Logradouro = updateRequest.Endereco.Endereco?.Logradouro;
                oldLead.Endereco.Endereco.Numero = updateRequest.Endereco.Endereco?.Numero;
                oldLead.Endereco.Endereco.Complemento = updateRequest.Endereco.Endereco?.Complemento;
                oldLead.Endereco.Endereco.Bairro = updateRequest.Endereco.Endereco?.Bairro;
                oldLead.Endereco.Endereco.CEP = updateRequest.Endereco.Endereco?.CEP;
                oldLead.Endereco.Endereco.Cidade = updateRequest.Endereco.Endereco?.Cidade != null ?
                    CidadeRepository.Get(updateRequest.Endereco.Endereco.Cidade.Id, true) : null;
                oldLead.Endereco.Tipo = TipoDeEndereco.Residencial;
            }

            return oldLead;
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[controller]/identificar")]
        public long? Identificar(string cpf)
        {
            var id = Repository.GetAll()
                .Where(i => i.Cpf == EncryptedText.Build(cpf))
                .OrderBy(i => i.Id)
                .Select(i => i.Id)
                .FirstOrDefault();
            return id > 0 ? id : null;
        }

            [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[controller]/precadastro")]
        public  string PreCadastro([Microsoft.AspNetCore.Mvc.FromBody] PreCadastroDto insertRequest)
        {
            var result = "";

            var lead = new Lead()
            {
                NomeCompleto = EncryptedText.Build(insertRequest.Nome),
                Cpf = EncryptedText.Build(insertRequest.Cpf),
                Email = EncryptedText.Build(insertRequest.Email),
                Cnh = EncryptedText.Build(insertRequest.Cnh),
                Observacao = insertRequest.Observacoes,
                EstadoCivil = !string.IsNullOrEmpty(insertRequest.EstadoCivil) ? (EstadoCivil)int.Parse(insertRequest.EstadoCivil) : null,
                Situacao = SituacaoDeLead.Ativo,
                DataDeCadastro = DateTime.Now,
                Sexo = (Sexo)int.Parse(insertRequest.Genero),
                Telefones = new List<TelefoneDePessoa>(),
                Enderecos = new List<EnderecoDePessoa>(),
                DataDeNascimento = DateTime.ParseExact(insertRequest.DataNascimento, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                DocumentoDeIdentidade = EncryptedText.Build(insertRequest.Rg),
            };

            if (insertRequest.Profissao != null)
            {
                lead.Profissao = insertRequest.Profissao != null ? ProfissaoRepository.Get(long.Parse(insertRequest.Profissao), true) : null;
            }

            if (!string.IsNullOrEmpty(insertRequest.Telefone))
            {
                var telefoneParts = insertRequest.Telefone.Split(")");
                var ddd = telefoneParts[0].Replace("(", "").Trim();
                var numero = telefoneParts[1].Replace("-", "").Trim();

                var telefone = new TelefoneDePessoa()
                {
                    Pessoa = lead,
                    Numero = EncryptedText.Build(numero),
                    DDD = int.Parse(ddd),
                    Tipo = TipoDeTelefone.Residencial,
                };
                lead.Telefones.Add(telefone);
            }

            if (!string.IsNullOrEmpty(insertRequest.Celular))
            {
                var telefoneParts = insertRequest.Celular.Split(")");
                var ddd = telefoneParts[0].Replace("(", "").Trim();
                var numero = telefoneParts[1].Replace("-", "").Trim();

                var celular = new TelefoneDePessoa()
                {
                    Pessoa = lead,
                    Numero = EncryptedText.Build(numero),
                    DDD = int.Parse(ddd),
                    Tipo = TipoDeTelefone.Celular,
                };
                lead.Telefones.Add(celular);
            }

            if (!string.IsNullOrEmpty(insertRequest.Cidade) && !string.IsNullOrEmpty(insertRequest.Estado))
            {
                var cidade = CidadeRepository.GetAll()
                    .Where(i => i.Nome == insertRequest.Cidade)
                    .Where(i => i.Estado.UF == insertRequest.Estado.ToUpper())
                    .FirstOrDefault();

                var endereco = new EnderecoDePessoa()
                {
                    Pessoa = lead,
                    Endereco = new Endereco()
                    {
                        Logradouro = insertRequest.Logradouro,
                        Numero = insertRequest.Numero,
                        Complemento = insertRequest.Complemento,
                        Bairro = insertRequest.Bairro,
                        CEP = insertRequest.Cep,
                        Cidade = cidade
                    },
                    Tipo = TipoDeEndereco.Residencial,
                };

                lead.Enderecos.Add(endereco);
            }

            base.Repository.Insert(lead);

            var content = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                name = lead.NomeCompleto.GetPlainText(),
                // email = lead.Email.GetPlainText(),
                cellPhone = $"{lead.Celular.Pais.Replace("+", "")}{lead.Celular.DDD}{lead.Celular.Numero.GetPlainText()}",
                employeeId = 34,
                tagId = 0,
                originId = 16,
                campaignId = 1,
                campaignSlug = "",
                observation = "",
                adCampaignName = "Pré cadastro",
                adSetName = "",
                adName = ""
            });


            WebRequestHelper.Execute(
                new CommunicationParameters()
                {
                    IgnoreCertificateErrors = true,
                    ReceiveTimeout = 30000,
                    SendTimeout = 30000,
                    TlsSslVersion = System.Net.SecurityProtocolType.Tls12,
                    UseDefaultProxy = true
                },
                new WebRequestParams()
                {
                    Accept = "application/json",
                    ContentType = "application/json",
                    Method = "POST",
                    Url = "https://api.unobject.com.br/v1/lead",
                },
                new WebRequestHeaders(
                    new WebRequestHeader("x-uno-access-token", "19DA1AF6DA5C013C4380"),
                    new WebRequestHeader("x-uno-secret-key", "c20b0285aca2674fb79f37b776c454e5d4a37ad9b6ceeeeebc")
                ),
                content,
                (response) =>
                {
                    var unoResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<UnoResponse>(response);

                    if (unoResponse.deal == null)
                    {
                        result = $"Cadastro realizado com a seguinte observação na integração com o Uno:\n\n {unoResponse.message}";
                    }

                    Console.WriteLine(response);

                }
            );

            return  result;
        }
    }

    public class LeadGetParams : IId
    {
        public long? Id { get; set; }
        public string Searchable { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public Tipo<SituacaoDeLead> Situacao { get; set; }
        public string Celular { get; set; }
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
        public TelefoneDePessoaDto Celular { get; set; }
        public EnderecoDePessoaDto Endereco { get; set; }
        public EntityReference Profissao { get; set; }
        public EntityReference Foto { get; set; }
        public Tipo<Sexo> Sexo { get; set; }
        public DateTime? DataDeNascimento { get; set; }
        public string DocumentoDeIdentidade { get; set; }
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
        public TelefoneDePessoaDto Celular { get; set; }
        public EnderecoDePessoaDto Endereco { get; set; }
        public EntityReference Profissao { get; set; }
        public EntityReference Foto { get; set; }
        public Tipo<Sexo> Sexo { get; set; }
        public DateTime? DataDeNascimento { get; set; }
        public string DocumentoDeIdentidade { get; set; }
    }

    public class PreCadastroDto
    {
        public string Celular { get; set; }
        public string Nome { get; set; }
        public string Genero { get; set; }
        public string DataNascimento { get; set; }
        public string EstadoCivil { get; set; }
        public string Profissao { get; set; }
        public string Nacionalidade { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
        public string Rg { get; set; }
        public string Cnh { get; set; }
        public string Pais { get; set; }
        public string Cep { get; set; }
        public string Bairro { get; set; }
        public string Estado { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Cidade { get; set; }
        public string Observacoes { get; set; }
    }



    public class UnoResponse
    {
        public string message { get; set; }
        public Deal deal { get; set; }
    }

    public class Deal
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string cellPhone { get; set; }
        public object[] dealTags { get; set; }
        public Dealobservation[] dealObservations { get; set; }
        public object rating { get; set; }
        public int employeeId { get; set; }
        public object referralId { get; set; }
        public string adCampaignName { get; set; }
        public object adSetName { get; set; }
        public object adName { get; set; }
        public object facebookSourceId { get; set; }
        public object facebookWaclId { get; set; }
        public DateTime createdAt { get; set; }
    }

    public class Dealobservation
    {
        public int id { get; set; }
        public string description { get; set; }
        public int employeeId { get; set; }
        public DateTime createdAt { get; set; }
    }

}
