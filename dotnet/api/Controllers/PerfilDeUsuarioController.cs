using System.Linq;
using framework;
using model;
using model.Repositories;
using api.Dtos;
using Microsoft.AspNetCore.Components;
using framework.Extensions;
using System.Collections.Generic;
using data;

namespace api.Controllers
{
    [Route("perfildeusuario/[action]")]
    [Route("perfildeusuario")]
    public class PerfilDeUsuarioController :
        EntityFastController<
            PerfilDeUsuario,
            PerfilDeUsuarioDto,
            PerfilDeUsuarioFast,
            PerfilDeUsuarioFastDto,
            PerfilDeUsuarioGetParams,
            PerfilDeUsuarioPostParams,
            PerfilDeUsuarioPutParams>
    {
        protected override UserAction UserActionForInsert => UserAction.InseriuPerfilDeUsuario;
        protected override UserAction UserActionForUpdate => UserAction.AlterouPerfilDeUsuario;
        protected override UserAction UserActionForDelete => UserAction.ExcluiuPerfilDeUsuario;

        private readonly PerfilDeUsuarioRepository PerfilDeUsuarioRepository;
        private readonly RotinaDoSistemaRepository RotinaDoSistemaRepository;

        public PerfilDeUsuarioController(
            PerfilDeUsuarioRepository repository,
            Repository<PerfilDeUsuarioFast> fastRepository,
            
            IAppContext appContext,
            RotinaDoSistemaRepository rotinaDoSistemaRepository
        ) : base(repository, fastRepository, appContext)
        {
            PerfilDeUsuarioRepository = repository;
            RotinaDoSistemaRepository = rotinaDoSistemaRepository;
        }

        protected override IQueryable<PerfilDeUsuario> Get(PerfilDeUsuarioGetParams getParams)
        {
            var usuarioLogado = AppContext.Usuario();

            var result = Repository.GetAll();

            if (getParams.Id.HasValue)
            {
                result = result.Where(i => i.Id == getParams.Id.Value);
            }
            if (!string.IsNullOrEmpty(getParams.Searchable))
            {
                result = result.Where(i => i.Searchable.Contains(getParams.Searchable));
            }
            if (getParams.TipoDePerfil != null)
            {
                result = result.Where(i => i.TipoDePerfil == getParams.TipoDePerfil);
            }
            return result;
        }

        protected override IQueryable<PerfilDeUsuarioFast> FastGet(PerfilDeUsuarioGetParams getParams)
        {
            var usuarioLogado = AppContext.Usuario();

            var result = FastRepository.GetAll()
                .Where(i => i.Situacao != SituacaoDePerfilDeUsuario.Excluido);

            if (getParams.Id.HasValue)
            {
                result = result.Where(i => i.Id == getParams.Id.Value);
            }
            if (!string.IsNullOrEmpty(getParams.Searchable))
            {
                result = result.Where(i => i.Searchable.Contains(
                    SearchableHelper.Build(getParams.Searchable, PerfilDeUsuario.SearchableScope)));
            }
            if (getParams.TipoDePerfil != null)
            {
                result = result.Where(i => i.TipoDePerfil == getParams.TipoDePerfil);
            }
            return result;
        }

        protected override PerfilDeUsuarioDto Convert(PerfilDeUsuario entity)
        {
            var rotinas = RotinaDoSistemaRepository.GetAll();

            var acessos = new List<AcessoDePerfilDeUsuarioDto>();

            rotinas.ForEach(i =>
            {
                var item = AcessoDePerfilDeUsuarioDto.Build(i, entity.Acessos.Any(j => j.Rotina.Id == i.Id));
                acessos.Add(item);
            });

            var result = new PerfilDeUsuarioDto()
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Situacao = entity.Situacao,
                TipoDePerfil = entity.TipoDePerfil,
                Acessos = acessos.ToArray()
            };
            return result;
        }

        protected override PerfilDeUsuarioFastDto Convert(PerfilDeUsuarioFast entity)
        {
            var result = new PerfilDeUsuarioFastDto()
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Situacao = entity.Situacao,
                TipoDePerfil = entity.TipoDePerfil
            };
            return result;
        }

        protected override PerfilDeUsuario Convert(PerfilDeUsuarioPostParams request)
        {
            var perfilDeUsuario = new PerfilDeUsuario();
            perfilDeUsuario.Nome = request.Nome;
            perfilDeUsuario.Situacao = request.Situacao;
            perfilDeUsuario.TipoDePerfil = request.TipoDePerfil;
            perfilDeUsuario.Acessos = new List<AcessoDePerfilDeUsuario>();

            request.Acessos.Where(i => i.AcessoPermitido).ForEach(i =>
            {
                var acesso = new AcessoDePerfilDeUsuario()
                {
                    PerfilDeUsuario = perfilDeUsuario,
                    Rotina = PerfilDeUsuarioRepository.GetRotina(i.Rotina.Id, true),
                };
                perfilDeUsuario.Acessos.Add(acesso);
            });

            return perfilDeUsuario;
        }

        protected override PerfilDeUsuario Convert(PerfilDeUsuarioPutParams request, PerfilDeUsuario perfilDeUsuario)
        {
            perfilDeUsuario.Nome = request.Nome;
            perfilDeUsuario.Situacao = request.Situacao;           
            perfilDeUsuario.TipoDePerfil = request.TipoDePerfil;
            perfilDeUsuario.Acessos.Merge(
                request.Acessos.Where(i => i.AcessoPermitido),
                ((i, j) => i.Rotina.Id == j.Rotina.Id),
                (i =>
                {
                    perfilDeUsuario.Acessos.Remove(i);
                    PerfilDeUsuarioRepository.Delete(i);
                }),
                null,
                (j =>
                {
                    var acesso = new AcessoDePerfilDeUsuario()
                    {
                        PerfilDeUsuario = perfilDeUsuario,
                        Rotina = PerfilDeUsuarioRepository.GetRotina(j.Rotina.Id, true),
                    };
                    perfilDeUsuario.Acessos.Add(acesso);
                })
            );
            return perfilDeUsuario;
        }
    }

    public class PerfilDeUsuarioGetParams : IId
    {
        public long? Id { get; set; }
        public string Searchable { get; set; }
        public Tipo<TipoDePerfilDeUsuario> TipoDePerfil { get; set; }
    }

    public class PerfilDeUsuarioPostParams : IPostParams
    {
        public string Nome { get; set; }
        public Tipo<SituacaoDePerfilDeUsuario> Situacao { get; set; }
        public Tipo<TipoDePerfilDeUsuario> TipoDePerfil { get; set; }
        public AcessoDePerfilDeUsuarioDto[] Acessos { get; set; }
    }

    public class PerfilDeUsuarioPutParams : IPutParams
    {
        public long? Id { get; set; }
        public string Nome { get; set; }
        public Tipo<SituacaoDePerfilDeUsuario> Situacao { get; set; }
        public Tipo<TipoDePerfilDeUsuario> TipoDePerfil { get; set; }
        public AcessoDePerfilDeUsuarioDto[] Acessos { get; set; }
    }
}
