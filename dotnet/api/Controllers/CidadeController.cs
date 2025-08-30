using api.Dtos;
using data;
using framework;
using model;
using System;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace api.Controllers
{
    [Route("cidade/[action]")]
    [Route("cidade")]
    public class CidadeController :
                EntityController<
                    Cidade,
                    CidadeDto,
                    CidadeGetParams,
                    CidadePostParams,
                    CidadePutParams>
    {
        protected override UserAction UserActionForInsert => throw new NotImplementedException();
        protected override UserAction UserActionForUpdate => throw new NotImplementedException();
        protected override UserAction UserActionForDelete => throw new NotImplementedException();

        private readonly Repository<Cidade> CidadeRepository;

        public CidadeController(
            Repository<Cidade> repository, 
            IAppContext appContext
        ) : base(repository, appContext)
        {
            CidadeRepository = repository;
        }
        
        protected override IQueryable<Cidade> Get(CidadeGetParams getParams)
        {
            var result = CidadeRepository.GetAll();
            if (!string.IsNullOrEmpty(getParams.Localidade) && !string.IsNullOrEmpty(getParams.Uf))
            {
                result = result.Where(i => i.Nome.ToUpper() == getParams.Localidade)
                               .Where(i => i.Estado.UF.ToUpper() == getParams.Uf);
            }
            if (!string.IsNullOrEmpty(getParams.Searchable))
            {
                result = result.Where(i => i.Searchable.Contains(SearchableHelper.Build(getParams.Searchable, Cidade.SearchableScope)));
            }
            return result;            
        }

        protected override CidadeDto Convert(Cidade entity)
        {
            var result = CidadeDto.Build(entity);
            return result;
        }

        protected override Cidade Convert(CidadePostParams insertRequest)
        {
            return null;
        }

        protected override Cidade Convert(CidadePutParams updateRequest, Cidade oldEntity)
        {
            return null;
        }
    }

    public class CidadeGetParams : IId
    {
        public long? Id { get; set; }
        public string Searchable { get; set; }
        public string Uf { get; set; }
        public string Localidade { get; set; }            
    }

    public class CidadePostParams : IPostParams
    {        
    }

    public class CidadePutParams : IPutParams
    {
        public long? Id { get; set; }
    }
}

