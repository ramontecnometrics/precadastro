using api.Dtos;
using data;
using framework;
using model;
using System.Linq;
using RouteAttribute = Microsoft.AspNetCore.Components.RouteAttribute;

namespace api.Controllers
{
    public class UnidadeGetParams : IId
    {
        public long? Id { get; set; }
        public string Searchable { get; set; }
    }

    public class UnidadePostParams : IPostParams
    {
        public string Nome { get; set; }
        public string Uuid { get; set; }
        public string UnoSecretKey { get; set; }
        public string UnoAccessToken { get; set; }
    }

    public class UnidadePutParams : IPutParams
    {
        public long? Id { get; set; }
        public string Nome { get; set; }
        public string UnoSecretKey { get; set; }
        public string UnoAccessToken { get; set; }
    }

    [Route("[controller]")]
    public class UnidadeController : EntityController<
        Unidade,
        UnidadeDto,
        UnidadeGetParams,
        UnidadePostParams,
        UnidadePutParams
    >
    {
        public UnidadeController(Repository<Unidade> repository, IAppContext appContext)
            : base(repository, appContext)
        {
        }

        protected override IQueryable<Unidade> Get(UnidadeGetParams getParams)
        {
            var query = Repository.GetAll();
            if (getParams.Id.HasValue)
            {
                query = query.Where(x => x.Id == getParams.Id.Value);
            }
            if (!string.IsNullOrEmpty(getParams.Searchable))
            {
                query = query.Where(x => x.Searchable == SearchableHelper.Build(getParams.Searchable, Unidade.SearchableScope));
            }
            return query;
        }

        protected override UnidadeDto Convert(Unidade entity)
        {
            return UnidadeDto.Build(entity);
        }

        protected override Unidade Convert(UnidadePostParams insertRequest)
        {
            return new Unidade()
            {
                Nome = insertRequest.Nome,
                Uuid = insertRequest.Uuid,
                UnoSecretKey = EncryptedText.Build(insertRequest.UnoSecretKey),
                UnoAccessToken = EncryptedText.Build(insertRequest.UnoAccessToken)
            };
        }

        protected override Unidade Convert(UnidadePutParams updateRequest, Unidade oldEntity)
        {
            oldEntity.Nome = updateRequest.Nome;
            oldEntity.UnoSecretKey = EncryptedText.Build(updateRequest.UnoSecretKey);
            oldEntity.UnoAccessToken = EncryptedText.Build(updateRequest.UnoAccessToken);
            return oldEntity;
        }

        protected override UserAction UserActionForInsert => UserAction.InseriuUnidade;
        protected override UserAction UserActionForUpdate => UserAction.AlterouUnidade;
        protected override UserAction UserActionForDelete => UserAction.ExcluiuUnidade;
    }
}
