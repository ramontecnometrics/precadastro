using api.Dtos;
using data;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentNHibernate.Conventions;
using framework;
using framework.Extensions;
using framework.Validators;
using Microsoft.AspNetCore.Mvc;
using model;
using model.Repositories;
using Newtonsoft.Json.Converters;
using Org.BouncyCastle.Asn1.Ocsp;
using Remotion.Linq.Clauses.ResultOperators;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
        private readonly Repository<TelefoneDePessoa> TelefoneDePessoaRepository;
        private readonly Repository<EnderecoDePessoa> EnderecoDePessoaRepository;
        private readonly Repository<Cidade> CidadeRepository;
        private readonly Repository<Unidade> UnidadeRepository;
        private readonly Repository<Formulario> FormularioRepository;
        private readonly ParametroDoSistemaRepository ParametroDoSistemaRepository;
        private readonly Repository<ResultadoDeAvaliacaoClinica> ResultadoDeAvaliacaoClinicaRepository;
        private readonly Repository<CampoDeGrupoDeFormulario> CampoDeGrupoDeFormularioRepository;
        private readonly Repository<ResultadoDeFormulario> ResultadoDeFormularioRepository;
        private readonly Repository<ResultadoDeAnamnese> ResultadoDeAnamneseRepository;


        public LeadController(
            LeadRepository repository,
            Repository<LeadFast> fastRepository,
            Repository<Arquivo> arquivoRepository,
            Repository<TelefoneDePessoa> telefoneDePessoaRepository,
            Repository<EnderecoDePessoa> enderecoDePessoaRepository,
            IAppContext appContext,
            Repository<Cidade> cidadeRepository,
            Repository<Unidade> unidadeRepository,
            Repository<Formulario> formularioRepository,
            ParametroDoSistemaRepository parametroDoSistemaRepository,
            Repository<ResultadoDeAvaliacaoClinica> resultadoDeAvaliacaoClinicaRepository,
            Repository<CampoDeGrupoDeFormulario> campoDeGrupoDeFormularioRepository,
            Repository<ResultadoDeFormulario> resultadoDeFormularioRepository,
            Repository<ResultadoDeAnamnese> resultadoDeAnamneseRepository) :
            base(repository, fastRepository, appContext)
        {
            LeadRepository = repository;
            ArquivoRepository = arquivoRepository;
            TelefoneDePessoaRepository = telefoneDePessoaRepository;
            EnderecoDePessoaRepository = enderecoDePessoaRepository;
            CidadeRepository = cidadeRepository;
            UnidadeRepository = unidadeRepository;
            FormularioRepository = formularioRepository;
            ParametroDoSistemaRepository = parametroDoSistemaRepository;
            ResultadoDeAvaliacaoClinicaRepository = resultadoDeAvaliacaoClinicaRepository;
            CampoDeGrupoDeFormularioRepository = campoDeGrupoDeFormularioRepository;
            ResultadoDeFormularioRepository = resultadoDeFormularioRepository;
            ResultadoDeAnamneseRepository = resultadoDeAnamneseRepository;
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
            if (!CPFValidator.IsValid(insertRequest.Cpf))
            {
                throw new Exception("CPF inválido! Verifique se digitou corretamente.");
            }

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
                Profissao = insertRequest.Profissao,
                DocumentoDeIdentidade = EncryptedText.Build(insertRequest.DocumentoDeIdentidade),
            };

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
                var numero = insertRequest.Celular.Numero.Replace("-", "").Trim();

                if (numero.Length != 9)
                {
                    throw new Exception("Celular inválido! Verifique se digitou corretamente.");
                }

                var celular = new TelefoneDePessoa()
                {
                    Pessoa = lead,
                    Numero = EncryptedText.Build(numero),
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

            if (!CPFValidator.IsValid(updateRequest.Cpf))
            {
                throw new Exception("CPF inválido! Verifique se digitou corretamente.");
            }

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
            oldLead.Profissao = updateRequest.Profissao;
            oldLead.Foto = updateRequest.Foto != null
                ? ArquivoRepository.Get(updateRequest.Foto.Id, true)
                : null;

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

                var numero = updateRequest.Celular.Numero.Replace("-", "").Trim();

                if (numero.Length != 9)
                {
                    throw new Exception("Celular inválido! Verifique se digitou corretamente.");
                }

                oldLead.Celular.Numero = EncryptedText.Build(numero);
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
        [Microsoft.AspNetCore.Mvc.Route("[controller]/anamnese/parametros")]
        public ParametrosDeAnamneseDto GetParametrosParaAnamnese(string cpf)
        {
            var result = new ParametrosDeAnamneseDto();

            var leadId = Repository.GetAll()
                .Where(i => i.Cpf == EncryptedText.Build(cpf))
                .OrderBy(i => i.Id)
                .Select(i => new { i.Id })
                .FirstOrDefault();

            if (leadId != null)
            {
                var lead = Repository.Get(leadId.Id, true);
                result.IdDoLead = lead.Id;
                if (lead.TokenParaAnamnese.IsEmpty())
                {
                    lead.TokenParaAnamnese = Guid.NewGuid().ToString();
                }

                Repository.Update(lead);

                result.TokenParaAnamnese = lead.TokenParaAnamnese;

                if (lead.Sexo.Is(Sexo.Feminino))
                {
                    var fichaJson = ParametroDoSistemaRepository.GetString("FichaDeAnamneseParaGeneroFeminino");

                    if (string.IsNullOrEmpty(fichaJson))
                    {
                        throw new Exception("A ficha de anamnese ainda não foi definida nos parâmetros do sistema.");
                    }

                    var converter = new ExpandoObjectConverter();
                    dynamic parametro = null;

                    parametro = Newtonsoft.Json.JsonConvert
                        .DeserializeObject<ExpandoObject>(fichaJson, converter);
                    long idDaFicha = parametro.id;

                    var ficha = FormularioRepository.Get(idDaFicha, true);

                    result.FichaDeAnamnese = FormularioDto.Build(ficha);
                }
                else
                {
                    var fichaJson = ParametroDoSistemaRepository.GetString("FichaDeAnamneseParaGeneroMasculino");

                    if (string.IsNullOrEmpty(fichaJson))
                    {
                        throw new Exception("A ficha de anamnese ainda não foi definida nos parâmetros do sistema.");
                    }

                    var converter = new ExpandoObjectConverter();
                    dynamic parametro = null;

                    parametro = Newtonsoft.Json.JsonConvert
                        .DeserializeObject<ExpandoObject>(fichaJson, converter);
                    long idDaFicha = parametro.id;

                    var ficha = FormularioRepository.Get(idDaFicha, true);

                    result.FichaDeAnamnese = FormularioDto.Build(ficha); 
                }
            }
            return result;
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[controller]/precadastro/parametros")]
        public ParametrosDePrecadastroDto GetParametrosParaPrecadastro(string id)
        {
            var result = new ParametrosDePrecadastroDto();

            var unidade = UnidadeRepository.GetAll()
                .Where(i => i.Uuid == id)
                .OrderBy(i => i.Id)
                .Select(i => new { i.Id, i.Nome })
                .FirstOrDefault();

            if (unidade != null)
            {
                result.Unidade = new UnidadeDto()
                {
                    Id = unidade.Id,
                    Nome = unidade.Nome,
                };
            }

            result.Unidades =
                UnidadeRepository.GetAll()
                .Select(i =>
                    new UnidadeDto()
                    {
                        Id = i.Id,
                        Nome = i.Nome,
                    }).ToArray();

            var fichaDeAvaliacaoClinicaParaGeneroMasculinoJson =
                ParametroDoSistemaRepository.GetString("FichaDeAvaliacaoClinicaParaGeneroMasculino");
            var fichaDeAvaliacaoClinicaParaGeneroFemininoJson =
                ParametroDoSistemaRepository.GetString("FichaDeAvaliacaoClinicaParaGeneroFeminino");

            if (string.IsNullOrEmpty(fichaDeAvaliacaoClinicaParaGeneroMasculinoJson))
            {
                throw new Exception("A ficha de avaliação clínica ainda não foi definida nos parâmetros do sistema.");
            }

            if (string.IsNullOrEmpty(fichaDeAvaliacaoClinicaParaGeneroFemininoJson))
            {
                throw new Exception("A ficha de avaliação clínica ainda não foi definida nos parâmetros do sistema.");
            }

            var converter = new ExpandoObjectConverter();
            dynamic parametro = null;

            parametro = Newtonsoft.Json.JsonConvert
                .DeserializeObject<ExpandoObject>(fichaDeAvaliacaoClinicaParaGeneroMasculinoJson, converter);
            long idDaFichaDeAvaliacaoClinicaParaGeneroMasculino = parametro.id;

            parametro = Newtonsoft.Json.JsonConvert
                .DeserializeObject<ExpandoObject>(fichaDeAvaliacaoClinicaParaGeneroFemininoJson, converter);
            long idDaFichaDeAvaliacaoClinicaParaGeneroFeminino = parametro.id;



            var fichaDeAvaliacaoClinicaParaGeneroMasculino =
                FormularioRepository.Get(idDaFichaDeAvaliacaoClinicaParaGeneroMasculino, true);
            var fichaDeAvaliacaoClinicaParaGeneroFeminino =
                FormularioRepository.Get(idDaFichaDeAvaliacaoClinicaParaGeneroFeminino, true);

            result.FichaDeAvaliacaoClinicaParaGeneroMasculino = FormularioDto.Build(fichaDeAvaliacaoClinicaParaGeneroMasculino);
            result.FichaDeAvaliacaoClinicaParaGeneroFeminino = FormularioDto.Build(fichaDeAvaliacaoClinicaParaGeneroFeminino);

            return result;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[controller]/precadastro")]
        public RespostaDeInclusaoDePreCadastroDto PreCadastro([Microsoft.AspNetCore.Mvc.FromBody] PreCadastroDto request)
        {
            var result = new RespostaDeInclusaoDePreCadastroDto();

            var urlDaApiDoUno = ParametroDoSistemaRepository.GetString("UrlDaApiDoUno");
            var codigoDoColaboradorParaIntegracaoComOUno = ParametroDoSistemaRepository.GetString("CodigoDoColaboradorParaIntegracaoComOUno");

            var unidade = UnidadeRepository.Get(request.IdDaUnidade, true);

            var leadExistente = Repository.GetAll()
                .Where(i => i.Cpf == EncryptedText.Build(request.Cpf))
                .Where(i => i.Situacao == SituacaoDeLead.Ativo)
                .FirstOrDefault();

            var lead = default(Lead);

            if (leadExistente != null)
            {
                lead = leadExistente;
                lead.Telefones.ToArray().ForEach(i =>
                {
                    lead.Telefones.Remove(i);
                    TelefoneDePessoaRepository.Delete(i);
                });

                lead.Enderecos.ToArray().ForEach(i =>
                {
                    lead.Enderecos.Remove(i);
                    EnderecoDePessoaRepository.Delete(i);
                });
            }
            else
            {
                lead = new Lead();
            }

            if (!CPFValidator.IsValid(request.Cpf))
            {
                throw new Exception("CPF inválido! Verifique se digitou corretamente.");
            }

            lead.NomeCompleto = EncryptedText.Build(request.Nome);
            lead.Cpf = EncryptedText.Build(request.Cpf);
            lead.Email = EncryptedText.Build(request.Email);
            lead.Cnh = EncryptedText.Build(request.Cnh);
            lead.Observacao = request.Observacoes;
            lead.Profissao = request.Profissao;
            lead.EstadoCivil = !string.IsNullOrEmpty(request.EstadoCivil) ? (EstadoCivil)int.Parse(request.EstadoCivil) : null;
            lead.Situacao = SituacaoDeLead.Ativo;
            lead.DataDeCadastro = DateTime.Now;
            lead.Sexo = (Sexo)int.Parse(request.Genero);
            lead.Telefones = new List<TelefoneDePessoa>();
            lead.Enderecos = new List<EnderecoDePessoa>();
            lead.DataDeNascimento = DateTime.ParseExact(request.DataNascimento, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            lead.DocumentoDeIdentidade = EncryptedText.Build(request.Rg);
            lead.TokenParaAvaliacaoClinica = Guid.NewGuid().ToString();
            lead.TokenParaAnamnese = Guid.NewGuid().ToString();
            lead.Unidade = unidade;

            if (!string.IsNullOrEmpty(request.Telefone))
            {
                var telefoneParts = request.Telefone.Split(")");
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

            if (!string.IsNullOrEmpty(request.Celular))
            {
                var telefoneParts = request.Celular.Split(")");
                var ddd = telefoneParts[0].Replace("(", "").Trim();
                var numero = telefoneParts[1].Replace("-", "").Trim();

                if (numero.Length != 9)
                {
                    throw new Exception("Celular inválido! Verifique se digitou corretamente.");
                }

                var celular = new TelefoneDePessoa()
                {
                    Pessoa = lead,
                    Numero = EncryptedText.Build(numero),
                    DDD = int.Parse(ddd),
                    Tipo = TipoDeTelefone.Celular,
                };
                lead.Telefones.Add(celular);
            }

            if (!string.IsNullOrEmpty(request.Cidade) && !string.IsNullOrEmpty(request.Estado))
            {
                var cidade = CidadeRepository.GetAll()
                    .Where(i => i.Nome == request.Cidade)
                    .Where(i => i.Estado.UF == request.Estado.ToUpper())
                    .FirstOrDefault();

                var endereco = new EnderecoDePessoa()
                {
                    Pessoa = lead,
                    Endereco = new Endereco()
                    {
                        Logradouro = request.Logradouro,
                        Numero = request.Numero,
                        Complemento = request.Complemento,
                        Bairro = request.Bairro,
                        CEP = request.Cep,
                        Cidade = cidade
                    },
                    Tipo = TipoDeEndereco.Residencial,
                };

                lead.Enderecos.Add(endereco);
            }

            if (leadExistente == null)
            {
                Repository.Insert(lead);
            }
            else
            {
                Repository.Update(lead);
            }

            result.IdDoLead = lead.Id;
            result.TokenParaAvaliacaoClinica = lead.TokenParaAvaliacaoClinica;

            if (leadExistente == null &&
                !string.IsNullOrEmpty(urlDaApiDoUno) &&
                !string.IsNullOrEmpty(codigoDoColaboradorParaIntegracaoComOUno) &&
                !string.IsNullOrEmpty(unidade.UnoAccessToken.GetPlainText()) &&
                !string.IsNullOrEmpty(unidade.UnoSecretKey.GetPlainText()))
            {
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

                try
                {
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
                            Url = $"{urlDaApiDoUno}/v1/lead",
                        },
                        new WebRequestHeaders(
                            new WebRequestHeader("x-uno-access-token", unidade.UnoAccessToken.GetPlainText()),
                            new WebRequestHeader("x-uno-secret-key", unidade.UnoSecretKey.GetPlainText())
                        ),
                        content,
                        (response) =>
                        {
                            var unoResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<UnoResponse>(response);

                            if (unoResponse.deal == null)
                            {
                                result.Mensagem = $"Cadastro realizado com a seguinte observação na integração com o Uno:\n\n {unoResponse.message}";
                            }
                            else
                            {
                                lead.IdentificacaoNoUno = unoResponse.deal.id;
                                Repository.Update(lead);
                            }

                            Console.WriteLine(response);

                        }
                    );
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            return result;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[controller]/precadastro/avaliacaoclinica")]
        public RespostaDeInclusaoDeAvaliacaoClinicaDto AvaliacaoClinica([Microsoft.AspNetCore.Mvc.FromBody] AvaliacaoClinicaDto request,
            [Microsoft.AspNetCore.Mvc.FromHeader] string tokenParaAvaliacaoClinica)
        {
            var lead = Repository.Get(request.IdDoLead, true);

            if (lead.TokenParaAvaliacaoClinica != tokenParaAvaliacaoClinica)
            {
                throw new Exception("Acesso negado.");
            }

            if (request.FichaDeAvaliacao == null)
            {
                throw new Exception("Ficha não informada.");
            }

            if (request.FichaDeAvaliacao.Grupos == null)
            {
                throw new Exception("Grupos da ficha não informados.");
            }

            if (request.FichaDeAvaliacao.Grupos.Length == 0)
            {
                throw new Exception("Nenhum grupo informado.");
            }

            var avaliacaoClinica = new ResultadoDeAvaliacaoClinica()
            {
                Lead = lead,
                Data = DateTimeSync.Now,
                Itens = new List<ResultadoDeFormularioDeAvaliacaoClinica>()
            };

            request.FichaDeAvaliacao.Grupos.ForEach(grupo =>
            {
                grupo.Campos.ForEach(campo =>
                {
                    var resultado = new ResultadoDeFormulario()
                    {
                        Campo = CampoDeGrupoDeFormularioRepository.Get(campo.Id, true),
                        Valor = campo.Valor
                    };
                    ResultadoDeFormularioRepository.Insert(resultado);

                    avaliacaoClinica.Itens.Add(new ResultadoDeFormularioDeAvaliacaoClinica()
                    {
                        ResultadoDeFormulario = resultado,
                        ResultadoDeAvaliacaoClinica = avaliacaoClinica
                    });
                });
            });

            ResultadoDeAvaliacaoClinicaRepository.Insert(avaliacaoClinica);
            lead.ResultadoDeAvaliacaoClinica = avaliacaoClinica;
            Repository.Update(lead);

            var result = new RespostaDeInclusaoDeAvaliacaoClinicaDto();
            return result;
        }


        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("[controller]/precadastro/anamnese")]
        public RespostaDeInclusaoDeAnamneseDto Anamnese([Microsoft.AspNetCore.Mvc.FromBody] AnamneseDto request,
           [Microsoft.AspNetCore.Mvc.FromHeader] string tokenParaAnamnese)
        {
            var lead = Repository.Get(request.IdDoLead, true);

            if (lead.TokenParaAnamnese != tokenParaAnamnese)
            {
                throw new Exception("Acesso negado.");
            }

            if (request.FichaDeAnamnese == null)
            {
                throw new Exception("Ficha não informada.");
            }

            if (request.FichaDeAnamnese.Grupos == null)
            {
                throw new Exception("Grupos da ficha não informados.");
            }

            if (request.FichaDeAnamnese.Grupos.Length == 0)
            {
                throw new Exception("Nenhum grupo informado.");
            }

            var anamnese = new ResultadoDeAnamnese()
            {
                Lead = lead,
                Data = DateTimeSync.Now,
                Itens = new List<ResultadoDeFormularioDeAnamnese>()
            };

            request.FichaDeAnamnese.Grupos.ForEach(grupo =>
            {
                grupo.Campos.ForEach(campo =>
                {
                    var resultado = new ResultadoDeFormulario()
                    {
                        Campo = CampoDeGrupoDeFormularioRepository.Get(campo.Id, true),
                        Valor = campo.Valor
                    };
                    ResultadoDeFormularioRepository.Insert(resultado);

                    anamnese.Itens.Add(new ResultadoDeFormularioDeAnamnese()
                    {
                        ResultadoDeFormulario = resultado,
                        ResultadoDeAnamnese = anamnese
                    });
                });
            });

            ResultadoDeAnamneseRepository.Insert(anamnese);
            lead.ResultadoDeAnamnese = anamnese;
            Repository.Update(lead);

            var result = new RespostaDeInclusaoDeAnamneseDto();
            return result;
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
        public string Profissao { get; set; }
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
        public string Profissao { get; set; }
        public EntityReference Foto { get; set; }
        public Tipo<Sexo> Sexo { get; set; }
        public DateTime? DataDeNascimento { get; set; }
        public string DocumentoDeIdentidade { get; set; }
    }

    public class PreCadastroDto
    {
        public long IdDaUnidade { get; set; }
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

    public class ParametrosDePrecadastroDto
    {
        public UnidadeDto Unidade { get; set; }
        public FormularioDto FichaDeAvaliacaoClinicaParaGeneroMasculino { get; set; }
        public FormularioDto FichaDeAvaliacaoClinicaParaGeneroFeminino { get; set; }
        public UnidadeDto[] Unidades { get; set; }
    }

    public class ParametrosDeAnamneseDto
    {
        public long? IdDoLead { get; set; }
        public string TokenParaAnamnese { get; set; }
        public FormularioDto FichaDeAnamnese { get; set; }
    }

    public class RespostaDeInclusaoDePreCadastroDto
    {
        public long IdDoLead { get; set; }
        public string Mensagem { get; set; }
        public string TokenParaAvaliacaoClinica { get; set; }
    }

    public class AvaliacaoClinicaDto
    {
        public long IdDoLead { get; set; }
        public ResultadoDeFormularioDto FichaDeAvaliacao { get; set; }
    }

    public class RespostaDeInclusaoDeAvaliacaoClinicaDto
    {

    }

    public class GrupoDeResultadoDeAvaliacaoClinicaDto
    {
        public long Id { get; set; }
        public CampoDeResultadoDeAvaliacaoClinicaDto[] Campos { get; set; }
    }

    public class CampoDeResultadoDeAvaliacaoClinicaDto
    {
        public long Id { get; set; }
        public string Valor { get; set; }
    }


    public class AnamneseDto
    {
        public long IdDoLead { get; set; }
        public ResultadoDeFormularioDto FichaDeAnamnese { get; set; }
    }


    public class RespostaDeInclusaoDeAnamneseDto
    {

    }




}
