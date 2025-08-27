using framework;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using log;
using System;
using System.ComponentModel;
using System.Globalization;
using framework.Extensions;
using api.Dtos;
using data;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogController
    {
        private IAppContext appContext;

        public LogController(IAppContext appContext)
        {
            this.appContext = appContext;
        }

        [HttpGet]
        public virtual ActionResult<TGetResponse<Dtos.LogDataDto>> Get([FromQuery] GetParams request)
        {
            var logHelper = new LogHelper();
            return logHelper.Get(request);
        }
    }

    public class LogHelper
    {
        public virtual ActionResult<TGetResponse<Dtos.LogDataDto>> Get([FromQuery] GetParams request)
        {
            var actionResult = default(ActionResult<TGetResponse<Dtos.LogDataDto>>);
            Logger.GetAll((all) =>
            {
                if (request == null)
                {
                    request = new GetParams();
                }
                if (string.IsNullOrEmpty(request.OrderBy))
                {
                    request.OrderBy = "Id";
                }
                var result = new TGetResponse<LogDataDto>();

                var getParams = default(LogGetParams);
                if (request.Id.HasValue)
                {
                    getParams = new LogGetParams()
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
                    var item = Filtrar(all, getParams).FirstOrDefault();
                    var any = item != null;
                    result = new TGetResponse<LogDataDto>()
                    {
                        Count = any ? 1 : 0,
                        Items = any ? new LogDataDto[] { Convert(item) } : new LogDataDto[] { },
                        PageSize = any ? 1 : 0,
                    };
                }
                else
                if (request.PageSize.HasValue)
                {
                    var count = Filtrar(all, getParams).Count();
                    var skip = request.Skip.HasValue ? request.Skip.Value : 0;
                    var pageSize = request.PageSize.HasValue && request.PageSize.Value > 0 ?
                        request.PageSize.Value > count ?
                            count : request.PageSize.Value
                        : count;
                    result = new TGetResponse<LogDataDto>()
                    {
                        Count = count,
                        Items = Filtrar(all, getParams)
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
                    result = new TGetResponse<LogDataDto>()
                    {
                        Items = Filtrar(all, getParams)
                            .Order(request.OrderBy)
                            .Select(i => Convert(i))
                            .ToArray()
                    };
                    result.Count = result.Items.Count();
                    result.PageSize = result.Count;
                }
                actionResult = result;
            });
            return actionResult;
        }

        private IQueryable<LogData> Filtrar(IQueryable<LogData> all, LogGetParams getParams)
        {
            if (getParams.Id.HasValue)
            {
                all = all.Where(i => i.Id == getParams.Id.Value);
            }

            if (!string.IsNullOrEmpty(getParams.Searchable))
            {
                all = all.Where(i => i.Message.Contains(getParams.Searchable));
            }

            if (getParams.DataInicial.HasValue)
            {
                all = all.Where(i => i.Date >= getParams.DataInicial.Value);
            }

            if (getParams.DataFinal.HasValue)
            {
                var dataFinal = getParams.DataFinal.Value.AddDays(1).AddSeconds(-1);
                all = all.Where(i => i.Date <= dataFinal);
            }

            if (!string.IsNullOrWhiteSpace(getParams.Action))
            {
                all = all.Where(i => i.Action == getParams.Action);
            }

            return all;
        }

        protected virtual LogDataDto Convert(LogData entity)
        {
            var result = LogDataDto.Build(entity);
            return result;
        }

        protected virtual LogGetParams ParseQueryString(string queryString)
        {
            var obj = Activator.CreateInstance<LogGetParams>();
            if (!string.IsNullOrEmpty(queryString))
            {
                var properties = typeof(LogGetParams).GetProperties();
                foreach (var property in properties)
                {
                    var valueAsString = System.Web.HttpUtility.ParseQueryString(queryString)[property.Name];
                    object value = null;

                    try
                    {
                        value = Parse(valueAsString, property.PropertyType);
                    }
                    catch (System.Exception)
                    {
                        if (int.TryParse(valueAsString, out int valueAsInt))
                        {
                            if (typeof(framework.Tipo<>) == property.PropertyType.GetGenericTypeDefinition())
                            {
                                value = Activator.CreateInstance(property.PropertyType, valueAsInt);
                            }
                        }
                    }

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

    public class LogGetParams : IId
    {
        public long? Id { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public string Action { get; set; }
        public string Searchable { get; set; }
    }
}