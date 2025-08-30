

using api.Dtos;
using data;
using framework;
using Microsoft.AspNetCore.Components;
using model;
using System.Linq;

namespace api.Controllers
{
    [Route("profissao/[action]")]
    [Route("profissao")]
    public class ProfissaoController :
                ReadOnlyEntityController<
                    Profissao,
                    ProfissaoDto,
                    ProfissaoGetParams>
    {


        public ProfissaoController(
            Repository<Profissao> repository,
            IAppContext appContext
        ) : base(repository, appContext)
        {

        }

        protected override IQueryable<Profissao> Get(ProfissaoGetParams getParams)
        {
            var result = Repository.GetAll();
            if (getParams.Id.HasValue)
            {
                result = result.Where(i => i.Id == getParams.Id.Value);
            }

            if (!string.IsNullOrEmpty(getParams.Searchable))
            {
                result = result.Where(i => i.Searchable.Contains(SearchableHelper.Build(getParams.Searchable, Profissao.SearchableScope)));
            }

            return result;
        }

        protected override ProfissaoDto Convert(Profissao entity)
        {
            var result = ProfissaoDto.Build(entity);
            return result;
        }

    }

    public class ProfissaoGetParams : IId
    {
        public long? Id { get; set; }
        public string Searchable { get; set; }
        public string Codigo { get; set; }
    }
}

