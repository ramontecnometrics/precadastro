using model;
using System.Linq;

namespace api.Dtos
{
    public class FormularioDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }

        public GrupoDeFormularioDto[] Grupos { get; set; }

        public static FormularioDto Build(Formulario entity)
        {
            if (entity == null) return null;

            return new FormularioDto()
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Descricao = entity.Descricao,
                Grupos = entity.Grupos?.Select(GrupoDeFormularioDto.Build).ToArray()
            };
        }
    }

    public class FormularioSimplesDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }

        public static FormularioSimplesDto Build(Formulario entity)
        {
            if (entity == null) return null;

            return new FormularioSimplesDto()
            {
                Id = entity.Id,
                Nome = entity.Nome
            };
        }
    }

    public class FormularioFastDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }

        public static FormularioFastDto Build(FormularioFast entity)
        {
            if (entity == null) return null;

            return new FormularioFastDto()
            {
                Id = entity.Id,
                Nome = entity.Nome
            };
        }
    }

    public class GrupoDeFormularioDto
    {
        public long Id { get; set; }
        public int Ordem { get; set; }
        public string Titulo { get; set; }

        public CampoDeGrupoDeFormularioDto[] Campos { get; set; }

        public static GrupoDeFormularioDto Build(GrupoDeFormulario entity)
        {
            if (entity == null) return null;

            return new GrupoDeFormularioDto()
            {
                Id = entity.Id,
                Titulo = entity.Titulo,
                Campos = entity.Campos?.Select(CampoDeGrupoDeFormularioDto.Build).ToArray(),
                Ordem = entity.Ordem
            };
        }
    }

    public class CampoDeGrupoDeFormularioDto
    {
        public long Id { get; set; }
        public int Ordem { get; set; }
        public string Titulo { get; set; }
        public string Tipo { get; set; }
        public bool Obrigatorio { get; set; }

        public static CampoDeGrupoDeFormularioDto Build(CampoDeGrupoDeFormulario entity)
        {
            if (entity == null) return null;

            return new CampoDeGrupoDeFormularioDto()
            {
                Id = entity.Id,
                Titulo = entity.Titulo,
                Tipo = entity.Tipo,
                Obrigatorio = entity.Obrigatorio,
                Ordem = entity.Ordem
            };
        }
    }
}
