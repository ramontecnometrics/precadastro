using data;
using framework.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using framework.Security;
using System.Text;
using framework;
using System.Collections.Generic;
using model;

using Newtonsoft.Json;

namespace api.Controllers
{
    public class GetParams : IId
    {
        public long? Id { get; set; }
        public int? Skip { get; set; }
        public int? PageSize { get; set; }
        public string OrderBy { get; set; }
        public string Query { get; set; }
    }

    public interface IPostParams
    {
    }

    public interface IPutParams : IId
    {
    }

    public class DeleteParams : IId
    {
        public long? Id { get; set; }
    }

    public class TGetResponse<TDto>
    {
        public int PageSize { get; set; }
        public int Count { get; set; }
        public TDto[] Items { get; set; }
    }

    public class TFastGetResponse<TFastDto>
    {
        public int PageSize { get; set; }
        public int Count { get; set; }
        public TFastDto[] Items { get; set; }
    }

    public class ProtectedPost
    {
        public string[] Data { get; set; }
    }

    public class ProtectedPut
    {
        public string[] Data { get; set; }
    }

    [ApiController]
    public abstract class ReadOnlyEntityController<
        TEntity, /* Entidade a ser gerenciada pelo controller */
        TDto, /* Versão completa da entidade a ser retornada nas consultas */
        TGetParams /* Parâmetros utilizados no método GET */
    >
    : ControllerBase
        where TEntity : IEntity, new()
        where TGetParams : IId, new()
    {
        protected readonly IAppContext AppContext;
         
        protected IKeyProvider KeyProvider
        {
            get
            {
                return this.AppContext != null ? this.AppContext.KeyProvider : null;
            }
        }
        public Repository<TEntity> Repository { get; set; }

        public ReadOnlyEntityController(Repository<TEntity> repository, IAppContext appContext)
        {
            this.Repository = repository;
            AppContext = appContext;
        }

        [HttpGet]
        [Route("[controller]")]
        public virtual ActionResult<TGetResponse<TDto>> Get([FromQuery] GetParams request)
        {
            if (request == null)
            {
                request = new GetParams();
            }
            if (string.IsNullOrEmpty(request.OrderBy))
            {
                request.OrderBy = "Id";
            }
            var result = new TGetResponse<TDto>();

            TGetParams getParams = default(TGetParams);
            if (request.Id.HasValue)
            {
                getParams = new TGetParams()
                {
                    Id = request.Id.Value
                };
            }
            else
            {
                getParams = ControllerHelper.ParseQueryString<TGetParams>(request.Query);
            }
            if (getParams.Id.HasValue)
            {
                var item = Get(getParams).FirstOrDefault();
                var any = item != null;
                result = new TGetResponse<TDto>()
                {
                    Count = any ? 1 : 0,
                    Items = any ? new TDto[] { Convert(item) } : new TDto[] { },
                    PageSize = any ? 1 : 0,
                };
            }
            else
            if (request.PageSize.HasValue)
            {
                var count = Get(getParams).Count();
                var skip = request.Skip.HasValue ? request.Skip.Value : 0;
                var pageSize = request.PageSize.HasValue && request.PageSize.Value > 0 ?
                    request.PageSize.Value > count ?
                        count : request.PageSize.Value
                    : count;
                result = new TGetResponse<TDto>()
                {
                    Count = count,
                    Items = Get(getParams)
                                    .Order(request.OrderBy)
                                    .Skip(skip)
                                    .Take(pageSize)
                                    .ToArray()
                                    .Select(i => Convert(i))
                                    .ToArray(),
                };
                result.PageSize = result.Items.Count();
            }
            else
            {
                result = new TGetResponse<TDto>()
                {
                    Items = Get(getParams)
                        .Order(request.OrderBy)
                        .Select(i => Convert(i))
                        .ToArray()
                };
                result.Count = result.Items.Count();
                result.PageSize = result.Count;
            }
            return result;
        }

        // protected

        // Devem ser implementados nas classes herdeiras
        protected abstract IQueryable<TEntity> Get(TGetParams getParams);
        protected abstract TDto Convert(TEntity entity);

        protected virtual ActionResult<TGetResponse<TMyDto>> GetResponse<TMyEntity, TMyDto, TMyGetParams>(
            GetParams request,
            Func<TMyGetParams, IQueryable<TMyEntity>> queryable, Func<TMyEntity, TMyDto> convert
        ) where TMyGetParams : IId, new()
        {
            var result = ControllerHelper.GetResponse(request, queryable, convert);
            return result;
        }
    }

    [ApiController]
    public abstract class EntityController<
        TEntity, /* Entidade a ser gerenciada pelo controller */
        TDto, /* Versão completa da entidade a ser retornada nas consultas */
        TGetParams, /* Parâmetros utilizados no método GET */
        TPostParams, /* Parâmetros usados para realizar a inclusão de uma nova entidade */
        TPutParams /* Parâmetros utilizadas para realizar a atualização de uma entidade */
    >
    : ReadOnlyEntityController<TEntity, TDto, TGetParams>
        where TEntity : IEntity, new()
        where TGetParams : IId, new()
        where TPostParams : IPostParams, new()
        where TPutParams : IPutParams, new()
    {
        public EntityController(Repository<TEntity> repository, IAppContext appContext) :
            base(repository, appContext)
        {

        }

        [HttpPost]
        [Route("[controller]")]
        public virtual ActionResult<long> Post([FromBody] TPostParams request)
        {
            var entity = Convert(request);
            BeforePost(entity);
            Repository.Insert(entity);
            AfterPost(entity);
            AfterPost(entity, request);
            var result = entity.Id;
            AppContext.RegisterUserAction(UserActionForInsert, AuditoriaHelper.BuildDescriptionForInsert(entity));
            return result;
        }

        [HttpPut]
        [Route("[controller]")]
        public virtual ActionResult<bool> Put([FromBody] TPutParams request)
        {
            var entity = Repository.GetAll()
                .Where(i => i.Id == request.Id)
                .FirstOrDefault();
            if (entity == null)
            {
                throw  new Exception($"Usuário não encontrado: {request.Id}");
            }
            var serializerSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var entityBefore = JsonConvert.DeserializeObject<TEntity>(JsonConvert.SerializeObject(entity, serializerSettings));
            entity = Convert(request, entity);
            BeforePut(entity);
            Repository.Update(entity);
            AfterPut(entity, request);
            AppContext.RegisterUserAction(UserActionForUpdate,
                AuditoriaHelper.BuildDescriptionForUpdate(entityBefore, entity));
            return true;
        }

        [HttpDelete]
        [Route("[controller]")]
        public virtual ActionResult<bool> Delete([FromQuery] DeleteParams request)
        {
            var entity = Repository.Get(request.Id, true);
            BeforeDelete(entity);
            Repository.Delete(entity);
            AfterDelete(entity);
            AppContext.RegisterUserAction(UserActionForDelete, AuditoriaHelper.BuildDescriptionForDelete(entity));
            return true;
        }

        // protected

        // Devem ser implementados nas classes herdeiras        
        protected abstract TEntity Convert(TPostParams insertRequest);
        protected abstract TEntity Convert(TPutParams updateRequest, TEntity oldEntity);

        // Podem ser reimplementados nas classes herdeiras
        protected virtual void BeforePost(TEntity entity) { }
        protected virtual void AfterPost(TEntity entity) { }
        protected virtual void AfterPost(TEntity entity, TPostParams insertRequest) { }
        protected virtual void BeforePut(TEntity entity) { }
        protected virtual void AfterPut(TEntity entity, TPutParams updateRequest) { }
        protected virtual void BeforeDelete(TEntity entity) { }
        protected virtual void AfterDelete(TEntity entity) { }
        protected abstract UserAction UserActionForInsert { get; }
        protected abstract UserAction UserActionForUpdate { get; }
        protected abstract UserAction UserActionForDelete { get; }
    }

    [ApiController]
    public abstract class ProtectedEntityController<
        TEntity, /* Entidade a ser gerenciada pelo controller */
        TDto, /* Versão completa da entidade a ser retornada nas consultas */
        TGetParams, /* Parâmetros utilizados no método GET */
        TPostParams, /* Parâmetros usados para realizar a inclusão de uma nova entidade */
        TPutParams /* Parâmetros utilizadas para realizar a atualização de uma entidade */
    >
    : ControllerBase
        where TEntity : IEntity, new()
        where TGetParams : IId, new()
        where TPostParams : IPostParams, new()
        where TPutParams : IPutParams, new()
    {

        protected readonly Repository<TEntity> Repository;
        protected readonly IAppContext AppContext;
        
        protected IKeyProvider KeyProvider
        {
            get
            {
                return this.AppContext != null ? this.AppContext.KeyProvider : null;
            }
        }

        // public
        public ProtectedEntityController(Repository<TEntity> repository, IAppContext appContext)
        {
            this.Repository = repository;
            AppContext = appContext;
        }

        [HttpGet]
        [Route("[controller]")]
        public virtual ActionResult<TGetResponse<TDto>> Get([FromQuery] GetParams request)
        {
            if (request == null)
            {
                request = new GetParams();
            }
            if (string.IsNullOrEmpty(request.OrderBy))
            {
                request.OrderBy = "Id";
            }
            if (request == null)
            {
                request = new GetParams();
            }
            if (string.IsNullOrEmpty(request.OrderBy))
            {
                request.OrderBy = "Id";
            }
            var result = new TGetResponse<TDto>();

            TGetParams getParams = default(TGetParams);
            if (request.Id.HasValue)
            {
                getParams = new TGetParams()
                {
                    Id = request.Id.Value
                };
            }
            else
            {
                getParams = ParseQueryString(request.Query);
            }
            if (getParams.Id.HasValue)
            {
                var item = Get(getParams).FirstOrDefault();
                var any = item != null;
                result = new TGetResponse<TDto>()
                {
                    Count = any ? 1 : 0,
                    Items = any ? new TDto[] { Convert(item) } : new TDto[] { },
                    PageSize = any ? 1 : 0,
                };
            }
            else
            if (request.PageSize.HasValue)
            {
                var count = Get(getParams).Count();
                var skip = request.Skip.HasValue ? request.Skip.Value : 0;
                var pageSize = request.PageSize.HasValue && request.PageSize.Value > 0 ?
                    request.PageSize.Value > count ?
                        count : request.PageSize.Value
                    : count;
                result = new TGetResponse<TDto>()
                {
                    Count = count,
                    Items = Get(getParams)
                                    .Order(request.OrderBy)
                                    .Skip(skip)
                                    .Take(pageSize)
                                    .Select(i => Convert(i))
                                    .ToArray(),
                };
                result.PageSize = result.Items.Count();
            }
            else
            {
                result = new TGetResponse<TDto>()
                {
                    Items = Get(getParams)
                        .Order(request.OrderBy)
                        .Select(i => Convert(i))
                        .ToArray()
                };
                result.Count = result.Items.Count();
                result.PageSize = result.Count;
            }
            return result;
        }

        [HttpPost]
        [Route("[controller]")]
        public virtual ActionResult<long> Post([FromBody] ProtectedPost encryptedRequest)
        {
            var data = ControllerDataDecryptor.Decrypt(encryptedRequest.Data, AppContext.KeyProvider);
            var request = Newtonsoft.Json.JsonConvert.DeserializeObject<TPostParams>(data);
            var entity = Convert(request);
            BeforePost(entity);
            Repository.Insert(entity);
            AfterPost(entity);
            AfterPost(entity, request);
            var result = entity.Id;
            AppContext.RegisterUserAction(UserActionForInsert, AuditoriaHelper.BuildDescriptionForInsert(entity));
            return result;
        }


        [HttpPut]
        [Route("[controller]")]
        public virtual ActionResult<bool> Put([FromBody] ProtectedPut encryptedRequest)
        {
            var data = ControllerDataDecryptor.Decrypt(encryptedRequest.Data, AppContext.KeyProvider);
            var request = Newtonsoft.Json.JsonConvert.DeserializeObject<TPutParams>(data);
            var entity = Repository.Get(request.Id, true);
            var serializerSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var entityBefore = JsonConvert.DeserializeObject<TEntity>(JsonConvert.SerializeObject(entity, serializerSettings));
            entity = Convert(request, entity);
            BeforePut(entity);
            Repository.Update(entity);
            AfterPut(entity);
            AfterPut(entity, request);
            AppContext.RegisterUserAction(UserActionForUpdate,
                AuditoriaHelper.BuildDescriptionForUpdate(entityBefore, entity));
            return true;
        }

        [HttpDelete]
        [Route("[controller]")]
        public virtual ActionResult<bool> Delete([FromQuery] DeleteParams request)
        {
            var entity = Repository.Get(request.Id, true);
            BeforeDelete(entity);
            Repository.Delete(entity);
            AfterDelete(entity);
            AppContext.RegisterUserAction(UserActionForDelete, AuditoriaHelper.BuildDescriptionForDelete(entity));
            return true;
        }

        // protected

        // Devem ser implementados nas classes herdeiras
        protected abstract IQueryable<TEntity> Get(TGetParams getParams);
        protected abstract TDto Convert(TEntity entity);
        protected abstract TEntity Convert(TPostParams insertRequest);
        protected abstract TEntity Convert(TPutParams updateRequest, TEntity oldEntity);
        protected abstract UserAction UserActionForInsert { get; }
        protected abstract UserAction UserActionForUpdate { get; }
        protected abstract UserAction UserActionForDelete { get; }


        // Podem ser reimplementados nas classes herdeiras
        protected virtual void BeforePost(TEntity entity) { }
        protected virtual void AfterPost(TEntity entity) { }
        protected virtual void AfterPost(TEntity entity, TPostParams insertRequest) { }
        protected virtual void BeforePut(TEntity entity) { }
        protected virtual void AfterPut(TEntity entity) { }
        protected virtual void AfterPut(TEntity entity, TPutParams updateRequest) { }
        protected virtual void BeforeDelete(TEntity entity) { }
        protected virtual void AfterDelete(TEntity entity) { }

        protected virtual TGetParams ParseQueryString(string queryString)
        {
            var obj = Activator.CreateInstance<TGetParams>();
            if (!string.IsNullOrEmpty(queryString))
            {
                var properties = typeof(TGetParams).GetProperties();
                foreach (var property in properties)
                {
                    var valueAsString = System.Web.HttpUtility.ParseQueryString(queryString)[property.Name];
                    var value = Parse(valueAsString, property.PropertyType);

                    if (value == null)
                        continue;

                    property.SetValue(obj, value, null);
                }
            }
            return obj;
        }

        protected virtual object Parse(string valueToConvert, Type dataType)
        {
            var obj = TypeDescriptor.GetConverter(dataType);
            var value = obj.ConvertFromString(null, CultureInfo.InvariantCulture, valueToConvert);
            return value;
        }
    }

    [ApiController]
    public abstract class EntityFastController<
        TEntity, /* Entidade a ser gerenciada pelo controller */
        TDto, /* Versão completa da entidade a ser retornada nas consultas */
        TFastEntity, /* Versão simples da entidade, usada para buscas rápidas, sem joins ou demais complexidades */
        TFastDto, /* Versão simples da entidade a ser retornada nas consultas rápidas, como listagem em grades e nos combos */
        TGetParams, /* Parâmetros utilizados no método GET */
        TPostParams, /* Parâmetros usados para realizar a inclusão de uma nova entidade */
        TPutParams /* Parâmetros utilizadas para realizar a atualização de uma entidade */
    >
    : EntityController<TEntity, TDto, TGetParams, TPostParams, TPutParams>
        where TEntity : IEntity, new()
        where TFastEntity : IEntity, new()
        where TPostParams : IPostParams, new()
        where TPutParams : IPutParams, new()
        where TGetParams : IId, new()
    {
        public Repository<TFastEntity> FastRepository { get; set; }

        public EntityFastController(
            Repository<TEntity> repository,
            Repository<TFastEntity> fastRepository,
            IAppContext appContext)
        : base(repository, appContext)
        {
            this.FastRepository = fastRepository;
        }

        [Route("[controller]/fast")]
        [HttpGet]
        public virtual ActionResult<TFastGetResponse<TFastDto>> Fast([FromQuery] GetParams request)
        {
            if (request == null)
            {
                request = new GetParams();
            }
            if (string.IsNullOrEmpty(request.OrderBy))
            {
                request.OrderBy = "Id";
            }
            TFastGetResponse<TFastDto> result = null;
            TGetParams getParams = default(TGetParams);
            if (request.Id.HasValue)
            {
                getParams = new TGetParams()
                {
                    Id = request.Id.Value
                };
            }
            else
            {
                getParams = ControllerHelper.ParseQueryString<TGetParams>(request.Query);
            }
            if (getParams.Id.HasValue)
            {
                var item = FastGet(getParams).FirstOrDefault();
                var any = item != null;
                result = new TFastGetResponse<TFastDto>()
                {
                    Count = any ? 1 : 0,
                    Items = any ? new TFastDto[] { Convert(item) } : new TFastDto[] { },
                    PageSize = any ? 1 : 0,
                };
            }
            else
            if (request.PageSize.HasValue)
            {
                var count = FastGet(getParams).Count();
                var skip = request.Skip.HasValue ? request.Skip.Value : 0;
                var pageSize = request.PageSize.HasValue && request.PageSize.Value > 0 ?
                    request.PageSize.Value > count ?
                        count : request.PageSize.Value
                    : count;
                result = new TFastGetResponse<TFastDto>()
                {
                    Count = count,
                    Items = FastGet(getParams)
                                    .Order(request.OrderBy)
                                    .Skip(skip)
                                    .Take(pageSize)
                                    .Select(i => Convert(i))
                                    .ToArray(),
                };
                result.PageSize = result.Items.Count();
            }
            else
            {
                result = new TFastGetResponse<TFastDto>()
                {
                    Items = FastGet(getParams)
                        .Order(request.OrderBy)
                        .Select(i => Convert(i))
                        .ToArray()
                };
                result.Count = result.Items.Count();
                result.PageSize = result.Count;
            }
            return result;
        }

        protected abstract IQueryable<TFastEntity> FastGet(TGetParams getParams);
        protected abstract TFastDto Convert(TFastEntity entity);

    }

    internal static class ControllerDataDecryptor
    {
        public static string Decrypt(string[] data, IKeyProvider keyProvider)
        {
            if (data == null)
            {
                return null;
            }

            var dataBytes = new List<byte[]>();
            data.ForEach(i => dataBytes.Add(Convert.FromBase64String(i)));

            var privateKeyEncrypted = keyProvider.GetRSAPrivateKey().ToPlainText();
            var key = Encoding.UTF8.GetBytes(keyProvider.GetKey().ToPlainText());
            var iv = Encoding.UTF8.GetBytes(keyProvider.GetIV().ToPlainText());
            var privateKey = framework.Security.SymmetricEncryption.DecryptString(privateKeyEncrypted, key, iv);
            var decrypted = framework.Security.AsymmetricEncryption.Decrypt(privateKey, dataBytes.ToArray());
            var decryptedText = Encoding.UTF8.GetString(decrypted);
            return decryptedText;
        }
    }
}