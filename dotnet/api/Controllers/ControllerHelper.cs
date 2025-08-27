using data;
using framework.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;

namespace api.Controllers
{
    public static class ControllerHelper
    {
        public static ActionResult<TGetResponse<TMyDto>> GetResponse<TMyEntity, TMyDto, TMyGetParams>(
            GetParams request,
            Func<TMyGetParams, IQueryable<TMyEntity>> queryable, Func<TMyEntity, TMyDto> convert
        ) where TMyGetParams : IId, new()
        {
            if (request == null)
            {
                request = new GetParams();
            }
            // if (string.IsNullOrEmpty(request.OrderBy))
            // {
            //     request.OrderBy = "Id";
            // }
            var result = new TGetResponse<TMyDto>();

            var getParams = default(TMyGetParams);
            if (request.Id.HasValue)
            {
                getParams = new TMyGetParams()
                {
                    Id = request.Id.Value
                };
            }
            else
            {
                getParams = ParseQueryString<TMyGetParams>(request.Query);
            }
            if (getParams.Id.HasValue)
            {
                var item = queryable(getParams).FirstOrDefault();
                var any = item != null;
                result = new TGetResponse<TMyDto>()
                {
                    Count = any ? 1 : 0,
                    Items = any ? new TMyDto[] { convert(item) } : new TMyDto[] { },
                    PageSize = any ? 1 : 0,
                };
            }
            else
            if (request.PageSize.HasValue)
            {
                var count = queryable(getParams).Count();
                var skip = request.Skip.HasValue ? request.Skip.Value : 0;
                var pageSize = request.PageSize.HasValue && request.PageSize.Value > 0 ?
                    request.PageSize.Value > count ?
                        count : request.PageSize.Value
                    : count;
                result = new TGetResponse<TMyDto>()
                {
                    Count = count,
                    Items = string.IsNullOrEmpty(request.OrderBy) ?
                        queryable(getParams)
                                    .Skip(skip)
                                    .Take(pageSize)
                                    .Select(i => convert(i))
                                    .ToArray()
                        :
                        queryable(getParams)
                                    .Order(request.OrderBy)
                                    .Skip(skip)
                                    .Take(pageSize)
                                    .Select(i => convert(i))
                                    .ToArray()
                };
                result.PageSize = result.Items.Count();
            }
            else
            {
                result = new TGetResponse<TMyDto>()
                {
                    Items = string.IsNullOrEmpty(request.OrderBy) ?
                        queryable(getParams)
                            .Select(i => convert(i))
                            .ToArray()
                        :
                            queryable(getParams)
                            .Order(request.OrderBy)
                            .Select(i => convert(i))
                            .ToArray()
                };
                result.Count = result.Items.Count();
                result.PageSize = result.Count;
            }
            return result;
        }

        public static object Parse(string valueToConvert, Type dataType)
        {
            var obj = TypeDescriptor.GetConverter(dataType);
            var value = obj.ConvertFromString(null, CultureInfo.InvariantCulture, valueToConvert);
            return value;
        }

       public static string ToQueryString(NameValueCollection nvc)
        {
            return string.Join("&",
                nvc.AllKeys
                   .SelectMany(key => nvc.GetValues(key)
                                        .Select(value => $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}")));
        }

        public static TParams ParseQueryString<TParams>(string queryString)
        {
            var obj = Activator.CreateInstance<TParams>();
            if (!string.IsNullOrEmpty(queryString))
            {
                var properties = typeof(TParams).GetProperties();
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
                            if (property.PropertyType.IsGenericType && (typeof(framework.Tipo<>) == property.PropertyType.GetGenericTypeDefinition()))
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
    }
}