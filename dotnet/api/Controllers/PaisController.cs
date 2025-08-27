using api.Dtos;
using data;
using framework;
using Microsoft.AspNetCore.Components;
using model;
using System;
using System.Linq;

namespace api.Controllers
{
    [Route("pais/[action]")]
    [Route("pais")]
    public class PaisController :
                EntityController<
                    Pais,
                    PaisDto,
                    PaisGetParams,
                    PaisPostParams,
                    PaisPutParams>
    {
        protected override UserAction UserActionForInsert => throw new NotImplementedException();
        protected override UserAction UserActionForUpdate => throw new NotImplementedException();
        protected override UserAction UserActionForDelete => throw new NotImplementedException();

        private readonly Repository<Pais> PaisRepository;

        public PaisController(
            Repository<Pais> repository,
            IAppContext appContext
        ) : base(repository, appContext)
        {
            PaisRepository = repository;
        }

        protected override IQueryable<Pais> Get(PaisGetParams getParams)
        {
            var result = PaisRepository.GetAll();
            if (getParams.Id.HasValue)
            {
                result = result.Where(i => i.Id == getParams.Id.Value);
            }
            else
            {
                if (!string.IsNullOrEmpty(getParams.Codigo))
                {
                    result = result.Where(i => i.Codigo == getParams.Codigo);
                }
                if (!string.IsNullOrEmpty(getParams.Searchable))
                {
                    result = result.Where(i => i.Searchable.Contains(getParams.Searchable));
                }
            }
            return result;
        }

        protected override PaisDto Convert(Pais entity)
        {
            var result = PaisDto.Build(entity);
            return result;
        }

        protected override Pais Convert(PaisPostParams insertRequest)
        {
            return null;
        }

        protected override Pais Convert(PaisPutParams updateRequest, Pais oldEntity)
        {
            return null;
        }
    }

    public class PaisGetParams : IId
    {
        public long? Id { get; set; }
        public string Searchable { get; set; }
        public string Codigo { get; set; }
    }
    
    public class PaisPostParams : IPostParams
    {

    }

    public class PaisPutParams : IPutParams
    {
        public long? Id { get; set; }
    }
}

