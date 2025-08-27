using framework;
using model;
using api.Dtos;
using System.Collections.Generic;
using System;
using model.Repositories;
using data;

namespace api.Conversors
{

    public class SolicitacaoDeAlteracaoOuInclusaoDePessoa
    {
        public string NovaSenha { get; set; }
        public bool SenhaAlterada { get; set; }
        public DateTime? DataDeCadastro { get; set; }
        public string Apelido { get; set; }
        public string Cpf { get; set; }
        public string Cnpj { get; set; }
        public string DocumentoDeIdentidade { get; set; }
        public string OrgaoExpedidorDoDocumentoDeIdentidade { get; set; }
        public IEnumerable<EnderecoDePessoaDto> Enderecos { get; set; }
        public IEnumerable<TelefoneDto> Telefones { get; set; }
        public string Email { get; set; }
        public Tipo<Sexo> Sexo { get; set; }
        public Tipo<TipoDePessoa> TipoDePessoa { get; set; } 
        public ArquivoDto Foto { get; set; }
        public string NomeCompleto { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public DateTime? DataDeNascimento { get; set; }
        public bool EnviarNovaSenhaPorEmail { get; set; }
        public PaisDto Pais { get; set; }
    }

    public class PessoaConversor
    { 
        private readonly Repository<Arquivo> ArquivoRepository;
        private readonly PerfilDeUsuarioRepository PerfilDeUsuarioRepository;
        private readonly ParametroDoSistemaRepository ParametroDoSistemaRepository;
        private readonly EnvioDeEmail EnvioDeEmail; 
        private readonly EnderecoDePessoaConversor EnderecoDePessoaConversor;
        private readonly TelefoneDePessoaConversor TelefoneDePessoaConversor;
        private readonly Repository<Pais> PaisRepository;

        public PessoaConversor(
           IAppContext appContext, 
           Repository<Arquivo> arquivoRepository,
           PerfilDeUsuarioRepository perfilDeUsuarioRepository,
           ParametroDoSistemaRepository parametroDoSistemaRepository,
           EnvioDeEmail envioDeEmail, 
           EnderecoDePessoaConversor enderecoDePessoaConversor,
           TelefoneDePessoaConversor telefoneDePessoaConversor,
           Repository<Pais> paisRepository
           )
        { 
            ArquivoRepository = arquivoRepository;
            PerfilDeUsuarioRepository = perfilDeUsuarioRepository;
            ParametroDoSistemaRepository = parametroDoSistemaRepository;
            EnvioDeEmail = envioDeEmail; 
            EnderecoDePessoaConversor = enderecoDePessoaConversor;
            PaisRepository = paisRepository;
            TelefoneDePessoaConversor = telefoneDePessoaConversor;
        }

        public virtual void Fill<T, U>(T pessoa, U request)
            where T : Pessoa, new()
            where U : SolicitacaoDeAlteracaoOuInclusaoDePessoa
        {
            pessoa.Apelido = EncryptedText.Build(request.Apelido);
            pessoa.Cpf = EncryptedText.Build(request.Cpf);
            pessoa.Cnpj = EncryptedText.Build(request.Cnpj);
            pessoa.DocumentoDeIdentidade = EncryptedText.Build(request.DocumentoDeIdentidade);
            pessoa.OrgaoExpedidorDoDocumentoDeIdentidade = request.OrgaoExpedidorDoDocumentoDeIdentidade;
            pessoa.Pais = request.Pais != null ? PaisRepository.Get(request.Pais.Id, true) : null;
            pessoa.Email = EncryptedText.Build(request.Email);
            pessoa.TipoDePessoa = request.TipoDePessoa;
            pessoa.DataDeNascimento = request.DataDeNascimento;
            pessoa.Foto = request.Foto != null ? ArquivoRepository.Get(request.Foto.Id, true) : null;
            pessoa.NomeCompleto = EncryptedText.Build(request.NomeCompleto);
            pessoa.RazaoSocial = EncryptedText.Build(request.RazaoSocial);
            pessoa.NomeFantasia = EncryptedText.Build(request.NomeFantasia);
            pessoa.DataDeCadastro = request.DataDeCadastro.HasValue ? request.DataDeCadastro.Value : DateTimeSync.Now;
            pessoa.Sexo = request.Sexo != null ?
                request.Sexo : new Tipo<Sexo>(Sexo.NaoInformado); 
            EnderecoDePessoaConversor.Fill(pessoa, request.Enderecos);
            TelefoneDePessoaConversor.Fill(pessoa, request.Telefones); 
            
        }
    }
}
