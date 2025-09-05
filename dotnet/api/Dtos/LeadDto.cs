using api.Controllers;
using framework;
using model;
using System;

namespace api.Dtos
{
    public class LeadDto
    {
        public long Id { get; set; }
        public string NomeCompleto { get; set; }
        public string Cpf { get; set; }
        public string Cnpj { get; set; }
        public string Email { get; set; }
        public Tipo<SituacaoDeLead> Situacao { get; set; }
        public Tipo<EstadoCivil> EstadoCivil { get; set; }
        public string Cnh { get; set; }
        public string Observacao { get; set; }
        public string AlertaDeSaude { get; set; }
        public DateTime DataDeCadastro { get; set; }
        public TelefoneDePessoaDto Telefone { get; set; }
        public TelefoneDePessoaDto Celular { get; set; }
        public EnderecoDePessoaDto Endereco { get; set; }
        public string Profissao { get; set; }
        public ArquivoDto Foto { get; set; }
        public Tipo<Sexo> Sexo { get; set; }
        public DateTime? DataDeNascimento { get; set; }
        public string DocumentoDeIdentidade { get; set; }
        public ResultadoDeAvaliacaoClinicaDto AvaliacaoClinica { get; set; }
        public UnidadeDto Unidade { get; set; }
        public ResultadoDeAnamneseDto Anamnese { get; set; }

        public static LeadDto Build(Lead item)
        {
            var result = default(LeadDto);
            if (item != null)
            {
                result = new LeadDto()
                {
                    Id = item.Id,
                    Unidade = UnidadeDto.Build(item.Unidade),
                    NomeCompleto = item.NomeCompleto?.GetPlainText(),
                    Cpf = item.Cpf?.GetPlainText(),
                    Cnpj = item.Cnpj?.GetPlainText(),
                    Email = item.Email?.GetPlainText(),
                    Situacao = item.Situacao,
                    EstadoCivil = item.EstadoCivil,
                    Cnh = item.Cnh?.GetPlainText(),
                    DocumentoDeIdentidade = item.DocumentoDeIdentidade?.GetPlainText(),
                    Observacao = item.Observacao,
                    AlertaDeSaude = item.AlertaDeSaude,
                    DataDeCadastro = item.DataDeCadastro,
                    DataDeNascimento = item.DataDeNascimento,
                    Telefone = TelefoneDePessoaDto.Build(item.Telefone),
                    Celular = TelefoneDePessoaDto.Build(item.Celular),
                    Endereco = EnderecoDePessoaDto.Build(item.Endereco),
                    Profissao = item.Profissao,
                    Foto = ArquivoDto.Build(item.Foto),
                    Sexo = item.Sexo,
                    AvaliacaoClinica = ResultadoDeAvaliacaoClinicaDto.Build(item.ResultadoDeAvaliacaoClinica),
                    Anamnese = ResultadoDeAnamneseDto.Build(item.ResultadoDeAnamnese)
                };
            }
            return result;
        }
    }

    public class LeadFastDto
    {
        public long Id { get; set; }
        public string NomeCompleto { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public DateTime DataDeCadastro { get; set; }
        public string Celular { get; set; }
        public Tipo<SituacaoDeLead> Situacao { get; set; }
        public long IdDaUnidade { get; set; }
        public string NomeDaUnidade { get; set; }

        public static LeadFastDto Build(LeadFast item)
        {
            var result = default(LeadFastDto);
            if (item != null)
            {
                result = new LeadFastDto()
                {
                    Id = item.Id,
                    NomeCompleto = item.NomeCompleto?.GetPlainText(),
                    Cpf = item.Cpf?.GetPlainText(),
                    Email = item.Email?.GetPlainText(),
                    Telefone = item.Telefone?.NumeroComDDD,
                    Celular = item.Celular?.NumeroComDDD,
                    DataDeCadastro = item.DataDeCadastro,
                    Situacao = item.Situacao,
                    IdDaUnidade = item.IdDaUnidade,
                    NomeDaUnidade = item.NomeDaUnidade
                };
            }
            return result;
        }
    }
}
