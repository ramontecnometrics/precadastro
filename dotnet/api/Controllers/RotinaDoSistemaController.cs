using System.Linq;
using model;
using api.Dtos;
using Microsoft.AspNetCore.Components;
using data;
using framework;

namespace api.Controllers
{    
    [Route("[controller]")]
    public class RotinaDoSistemaController :
        EntityController<
            RotinaDoSistema,
            RotinaDoSistemaDto,
            RotinaDoSistemaGetParams,
            RotinaDoSistemaPostParams,
            RotinaDoSistemaPutParams>
    {
        protected override UserAction UserActionForInsert => UserAction.InseriuRotinaDoSistema;
        protected override UserAction UserActionForUpdate => UserAction.AlterouRotinaDoSistema;
        protected override UserAction UserActionForDelete => UserAction.ExcluiuRotinaDoSistema;

        public RotinaDoSistemaController(
            Repository<RotinaDoSistema> repository,            
            
            IAppContext appContext
        ) : base(repository, appContext)
        {

        }

        protected override IQueryable<RotinaDoSistema> Get(RotinaDoSistemaGetParams getParams)
        {
            var result = Repository.GetAll();
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

        protected override RotinaDoSistemaDto Convert(RotinaDoSistema entity)
        {
            var result = new RotinaDoSistemaDto()
            {
                Id = entity.Id,
                Descricao = entity.Descricao
            };
            return result;
        }

        protected override RotinaDoSistema Convert(RotinaDoSistemaPostParams insertRequest)
        {
            var RotinaDoSistema = new RotinaDoSistema();
            RotinaDoSistema.Descricao = insertRequest.Descricao;            
            return RotinaDoSistema;
        }

        protected override RotinaDoSistema Convert(RotinaDoSistemaPutParams updateRequest, RotinaDoSistema oldEntity)
        {
            oldEntity.Descricao = updateRequest.Descricao;
            return oldEntity;
        }
    }

    public class RotinaDoSistemaGetParams: IId
    {
        public long? Id { get; set; }
        public string Searchable { get; set; }
    }

    public class RotinaDoSistemaPostParams : IPostParams
    {
        public string Descricao { get; set; }
    }

    public class RotinaDoSistemaPutParams : IPutParams
    {
        public long? Id { get; set; }
        public string Descricao { get; set; } 
    }
}
