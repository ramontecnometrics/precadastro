using api.Dtos;
using data;
using framework;
using framework.Extensions;
using Microsoft.AspNetCore.Mvc;
using model;
using System.Collections.Generic;
using System.Linq;
using RouteAttribute = Microsoft.AspNetCore.Components.RouteAttribute;

namespace api.Controllers
{
    [ApiController]
    [Route("formulario/[action]")]
    [Route("formulario")]
    public class FormularioController : EntityFastController<
        Formulario,
        FormularioDto,
        FormularioFast,
        FormularioFastDto,
        FormularioGetParams,
        FormularioPostParams,
        FormularioPutParams>
    {
        private readonly Repository<GrupoDeFormulario> GrupoDeFormularioRepository;
        private readonly Repository<CampoDeGrupoDeFormulario> CampoDeGrupoDeFormularioRepository;

        public FormularioController(
            Repository<Formulario> repository,
            Repository<FormularioFast> fastRepository,
            IAppContext appContext,
            Repository<GrupoDeFormulario> grupoDeFormularioRepository,
            Repository<CampoDeGrupoDeFormulario> campoDeGrupoDeFormularioRepository)
            : base(repository, fastRepository, appContext)
        {
            GrupoDeFormularioRepository = grupoDeFormularioRepository;
            CampoDeGrupoDeFormularioRepository = campoDeGrupoDeFormularioRepository;
        }

        protected override IQueryable<Formulario> Get(FormularioGetParams getParams)
        {
            var result = Repository.GetAll();

            if (getParams.Id.HasValue)
            {
                result = result.Where(x => x.Id == getParams.Id.Value);
            }

            if (!string.IsNullOrEmpty(getParams.Searchable))
            {
                result = result.Where(i => i.Searchable.Contains(SearchableHelper.Build(getParams.Searchable, Formulario.SearchableScope)));
            }

            return result;
        }

        protected override IQueryable<FormularioFast> FastGet(FormularioGetParams getParams)
        {
            var result = FastRepository.GetAll();

            if (getParams.Id.HasValue)
            {
                result = result.Where(x => x.Id == getParams.Id.Value);
            }

            if (!string.IsNullOrEmpty(getParams.Searchable))
            {
                result = result.Where(i => i.Searchable.Contains(SearchableHelper.Build(getParams.Searchable, Formulario.SearchableScope)));
            }

            return result;
        }

        protected override FormularioDto Convert(Formulario entity)
        {
            var result = FormularioDto.Build(entity);
            return result;
        }

        protected override FormularioFastDto Convert(FormularioFast entity)
        {
            var result = FormularioFastDto.Build(entity);
            return result;
        }

        protected override Formulario Convert(FormularioPostParams insertRequest)
        {
            var entity = new Formulario()
            {
                Nome = insertRequest.Nome,
                Descricao = insertRequest.Descricao,
                Grupos = new List<GrupoDeFormulario>()
            };

            MergeGrupos(entity, entity.Grupos, insertRequest.Grupos);

            return entity;
        }

        protected override Formulario Convert(FormularioPutParams updateRequest, Formulario oldEntity)
        {
            oldEntity.Nome = updateRequest.Nome;
            oldEntity.Descricao = updateRequest.Descricao;
            MergeGrupos(oldEntity, oldEntity.Grupos, updateRequest.Grupos);
            return oldEntity;
        }

        protected virtual void MergeGrupos(Formulario parentEntity,
            IList<GrupoDeFormulario> grupos,
            IEnumerable<GrupoDeFormularioDto> gruposDto
        )
        {
            if (gruposDto == null)
            {
                return;
            }

            grupos.Merge(gruposDto,
                (entity, dto) => entity.Id == dto.Id,
                (entity) =>
                {
                    grupos.Remove(entity);
                    GrupoDeFormularioRepository.Delete(entity);
                },
                (entity, dto) =>
                {
                    entity.Titulo = dto.Titulo;
                    entity.Ordem = dto.Ordem;
                    MergeCampos(entity, entity.Campos, dto.Campos);
                },
                (dto) =>
                {
                    var entity = new GrupoDeFormulario()
                    {
                        Formulario = parentEntity,
                        Titulo = dto.Titulo,
                        Campos = new List<CampoDeGrupoDeFormulario>(),
                        Ordem = dto.Ordem
                    };
                    MergeCampos(entity, entity.Campos, dto.Campos);
                    grupos.Add(entity);
                });
        }

        protected virtual void MergeCampos(GrupoDeFormulario parentEntity,
            IList<CampoDeGrupoDeFormulario> campos,
            IEnumerable<CampoDeGrupoDeFormularioDto> camposDto)
        {
            if (camposDto == null)
            {
                return;
            }
            campos.Merge(camposDto,
                (entity, dto) => entity.Id == dto.Id,
                (entity) =>
                {
                    campos.Remove(entity);
                    CampoDeGrupoDeFormularioRepository.Delete(entity);
                },
                (entity, dto) =>
                {
                    entity.Titulo = dto.Titulo;
                    entity.Obrigatorio = dto.Obrigatorio;
                    entity.Tipo = dto.Tipo;
                    entity.Opcoes = dto.Opcoes;
                    entity.Ordem = dto.Ordem;
                },
                (dto) =>
                {
                    var entity = new CampoDeGrupoDeFormulario()
                    {
                        GrupoDeFormulario = parentEntity,
                        Obrigatorio = dto.Obrigatorio,
                        Tipo = dto.Tipo,
                        Opcoes = dto.Opcoes,
                        Titulo = dto.Titulo,
                        Ordem = dto.Ordem
                    };
                    campos.Add(entity);
                });
        }

        protected override UserAction UserActionForInsert => UserAction.FormularioInsert;
        protected override UserAction UserActionForUpdate => UserAction.FormularioUpdate;
        protected override UserAction UserActionForDelete => UserAction.FormularioDelete;
    }

    public class FormularioGetParams : IId
    {
        public long? Id { get; set; }
        public string Searchable { get; set; }
    }

    public class FormularioPostParams : IPostParams
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public GrupoDeFormularioDto[] Grupos { get; set; }
    }

    public class FormularioPutParams : IPutParams
    {
        public long? Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public GrupoDeFormularioDto[] Grupos { get; set; }
    }
}
