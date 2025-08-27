using framework;
using model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace api.Dtos
{
	public class UsuarioAdministradorDto
	{
		public long Id { get; set; }
		public string Email { get; set; }
		public Tipo<SituacaoDeUsuario> Situacao { get; set; }
		public ArquivoDto Foto { get; set; }
		public string Nome { get; set; }
		public IEnumerable<PerfilDoUsuarioDto> Perfis { get; set; }
		public string NomeDeUsuario { get; set; }
		public string Senha { get; set; }
		public string Certificado { get; set; }

        public static UsuarioAdministradorDto Build(UsuarioAdministrador item)
		{
			var result = default(UsuarioAdministradorDto);
			if (item != null)
			{
				result = new UsuarioAdministradorDto()
				{
					Id = item.Id,
					Email = item.Email.GetPlainText(),
					Foto = ArquivoDto.Build(item.Foto),
					Situacao = item.Situacao,
					Perfis = item.Perfis.Select(i => PerfilDoUsuarioDto.Build(i)),
					NomeDeUsuario = item.NomeDeUsuario,
					Senha = item.Senha != null && !string.IsNullOrEmpty(item.Senha.GetPlainText()) ? "********" : null,
					Nome = item.Nome.GetPlainText(),
					Certificado = item.Certificado.GetPlainText()
				};
			}
			return result;
		}
	}

	public class UsuarioAdministradorFastDto
	{
		public long Id { get; set; } 
		public string Email { get; set; }
		public string NomeDeUsuario { get; set; }
		public string Nome { get; set; } 
		public long IdDoUsuario { get; set; }

		public static UsuarioAdministradorFastDto Build(UsuarioAdministradorFast item)
		{
			var result = default(UsuarioAdministradorFastDto);
			if (item != null)
			{
				result = new UsuarioAdministradorFastDto()
				{
					Id = item.Id,
					Nome = item.Nome.GetPlainText(),
					Email = item.Email.GetPlainText(), 
					NomeDeUsuario = item.NomeDeUsuario,
					IdDoUsuario = item.IdDoUsuario,
				};
			}
			return result;
		}
	}

}

