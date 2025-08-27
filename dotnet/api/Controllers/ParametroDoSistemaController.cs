using System.Linq;
using model;
using api.Dtos;
using Microsoft.AspNetCore.Components;
using model.Repositories;
using framework;
using data;

namespace api.Controllers
{
    [Route("parametrodosistema/[action]")]
    [Route("parametrodosistema")]
    public class ParametroDoSistemaController :
        ProtectedEntityController<
            ParametroDoSistema,
            ParametroDoSistemaDto,
            ParametroDoSistemaGetParams,
            ParametroDoSistemaPostParams,
            ParametroDoSistemaPutParams>
    {
        protected override UserAction UserActionForInsert => UserAction.InseriuParametroDoSistema;
        protected override UserAction UserActionForUpdate => UserAction.AlterouParametroDoSistema;
        protected override UserAction UserActionForDelete => UserAction.ExcluiuParametroDoSistema;
 
        private readonly ParametroDoSistemaRepository ParametroDoSistemaRepository;

        public ParametroDoSistemaController(
            ParametroDoSistemaRepository repository, 
            
            IAppContext appContext
        ) : base(repository, appContext)
        { 
            ParametroDoSistemaRepository = repository;
        } 

        protected override IQueryable<ParametroDoSistema> Get(ParametroDoSistemaGetParams getParams)
        {
            var usuarioLogado = AppContext.Usuario();
            var result = Repository.GetAll();

            if (getParams.Id.HasValue)
            {
                result = result.Where(i => i.Id == getParams.Id.Value);
            }

            if (!string.IsNullOrWhiteSpace(getParams.Searchable))
            {
                result = result.ToArray()
                    .Where(i => i.Nome.Contains(getParams.Searchable, System.StringComparison.InvariantCultureIgnoreCase))
                    .ToList()
                    .AsQueryable();
            }

            return result;
        }

        protected override ParametroDoSistemaDto Convert(ParametroDoSistema entity)
        {
            var result = ParametroDoSistemaDto.Build(entity);
            return result;
        }

        protected override ParametroDoSistema Convert(ParametroDoSistemaPostParams insertRequest)
        {
            var item = new ParametroDoSistema();
            item.Descricao = insertRequest.Descricao;
            item.Nome = insertRequest.Nome;
            item.Grupo = insertRequest.Grupo;
            item.Ordem = insertRequest.Ordem;
            item.Protegido = insertRequest.Protegido;
            item.Valor = !string.IsNullOrEmpty(insertRequest.Valor) ?
                  framework.EncryptedText.Build(insertRequest.Valor) : null;
            return item;
        }

        protected override ParametroDoSistema Convert(ParametroDoSistemaPutParams updateRequest,
            ParametroDoSistema oldEntity)
        {
            oldEntity.Descricao = updateRequest.Descricao;
            oldEntity.Nome = updateRequest.Nome;
            oldEntity.Grupo = updateRequest.Grupo;
            oldEntity.Ordem = updateRequest.Ordem;
            oldEntity.Protegido = updateRequest.Protegido;
            oldEntity.Valor = !string.IsNullOrEmpty(updateRequest.Valor) ?
                  framework.EncryptedText.Build(updateRequest.Valor) : null;
            return oldEntity;
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[controller]/urlpublica")]
        public string GetUrlPublica()
        {
            return ParametroDoSistemaRepository.GetString("UrlPublica");
        }
    }

    public class ParametroDoSistemaGetParams : IId
    {
        public long? Id { get; set; }
        public string Searchable { get; set; }
    }

    public class ParametroDoSistemaPostParams : IPostParams
    {
        public int Grupo { get; set; }
        public int Ordem { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Valor { get; set; }
        public bool Protegido { get; set; }
    }

    public class ParametroDoSistemaPutParams : IPutParams
    {
        public long? Id { get; set; }
        public int Grupo { get; set; }
        public int Ordem { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Valor { get; set; }
        public bool Protegido { get; set; }
    }
}
