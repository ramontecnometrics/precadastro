using api.Dtos;
using data;
using framework;
using Microsoft.AspNetCore.Components;
using model;
using System;
using System.Linq;

namespace api.Controllers
{
    [Route("termodeuso/[action]")]
    [Route("termodeuso")]
    public class TermoDeUsoController :
           EntityController<
                 TermoDeUso,
                 TermoDeUsoDto,
                 TermoDeUsoGetParams,
                 TermoDeUsoPostParams,
                 TermoDeUsoPutParams>
    {
        protected override UserAction UserActionForInsert => UserAction.InseriuTermoDeUso;
        protected override UserAction UserActionForUpdate => throw new NotImplementedException();
        protected override UserAction UserActionForDelete => UserAction.ExcluiuTermoDeUso;

        public TermoDeUsoController(
            Repository<TermoDeUso> repository,
            
            IAppContext appContext
        ) : base(repository, appContext)
        {

        }

       protected override IQueryable<TermoDeUso> Get(TermoDeUsoGetParams getParams)
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

        protected override TermoDeUsoDto Convert(TermoDeUso entity)
        {
           var result = TermoDeUsoDto.Build(entity);
           return result;
        }

        protected override TermoDeUso Convert(TermoDeUsoPostParams insertRequest)
        {
            var termoDeUso = new TermoDeUso();
            termoDeUso.Nome = insertRequest.Nome;
            termoDeUso.Termo = insertRequest.Termo;
            termoDeUso.DataDeCadastro = DateTimeSync.Now.Date;
            return termoDeUso;
        }

        protected override TermoDeUso Convert(TermoDeUsoPutParams updateRequest, TermoDeUso oldEntity)
        {
            throw new Exception("Acesso negado à operação.");

        }
    }

    public class TermoDeUsoGetParams : IId
    {
        public long? Id { get; set; }
        public string Searchable { get; set; }
        public virtual DateTime DataDeCadastro { get; set; }
    }

    public class TermoDeUsoPostParams : IPostParams
    {
        public string Nome { get; set; }
        public string Termo { get; set; }
        public virtual DateTime DataDeCadastro { get; set; }
    }

    public class TermoDeUsoPutParams : IPutParams
    {
        public long? Id { get; set; }        
    }
}
