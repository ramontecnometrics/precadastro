using data;
using framework;
using framework.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace api.Middlewares
{
    public class RequestStartupMiddleware
    {
        private readonly RequestDelegate _next;
        private static string _HostName = null;

        public RequestStartupMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, UnitOfWorkScope scope)
        {
            if (_HostName == null)
            {
                _HostName = Dns.GetHostName();
            }

            var startTime = DateTimeSync.Now;
            var userIdHeader = httpContext.Request.Headers["UserId"].ToString();
            int? userId = null;

            if (int.TryParse(userIdHeader, out var userIdTemp))
            {
                userId = userIdTemp;
            }

            try
            {
                scope.StartTransaction();
                try
                {
                    httpContext.Response.Headers["ServerHostName"] = _HostName;
                    await _next(httpContext);
                    scope.Commit();                    
                }
                catch (Exception e)
                {
                    scope.Rollback();
                    log.Logger.Add(
                       Log.Modulo.WebApi.Description(),
                       httpContext.Request.Path.ToString().ToLower(),
                       httpContext.Request.QueryString.HasValue ? httpContext.Request.QueryString.Value : null,
                       e,
                       Log.Categoria.Geral.ToInt(),
                       Log.SubCategoria.Erro.ToInt(),
                       httpContext.TraceIdentifier,
                       httpContext.Request.Method,
                       httpContext.Connection.RemoteIpAddress,
                       startTime,
                       null,
                       userId);

                    throw;
                }
                finally
                {
                    scope.CloseSession();
                }
            }
            catch (Exception e)
            {
                log.Logger.Add(
                   Log.Modulo.WebApi.Description(),
                   httpContext.Request.Path.ToString().ToLower(),
                   httpContext.Request.QueryString.HasValue ? httpContext.Request.QueryString.Value : null,
                   e,
                   Log.Categoria.Geral.ToInt(),
                   Log.SubCategoria.Erro.ToInt(),
                   httpContext.TraceIdentifier,
                   httpContext.Request.Method,
                   httpContext.Connection.RemoteIpAddress,
                   startTime,
                   null,
                   userId);
                throw;
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AuthenticationMidllewareExtensions
    {
        public static IApplicationBuilder UseRequestStartupMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestStartupMiddleware>();
        }
    }
}
