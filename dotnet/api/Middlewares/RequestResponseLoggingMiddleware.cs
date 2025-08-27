using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
// Incompatibilidade com o 3.1
//using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using framework.Extensions;
using framework;
using model;

namespace api.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private static List<ModuloSemLog> _moduloSemLog = null;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IHttpContextAccessor contextAccessor)
        {
            var startTime = DateTimeSync.Now;
            var userIdHeader = contextAccessor.HttpContext.Request.Headers["UserId"].ToString();
            int? userId = null;

            if (int.TryParse(userIdHeader, out var userIdTemp))
            {
                userId = userIdTemp;
            }

            var gerarLog = GerarLog(contextAccessor.HttpContext.Request.Path, contextAccessor.HttpContext.Request.Method);
            if (gerarLog)
            {

                // var request = await FormatRequest(contextAccessor.HttpContext.Request);

                // Não gravar o payload

                var request = default(string);

                log.Logger.Add(
                    Log.Modulo.WebApi.Description(),
                    contextAccessor.HttpContext.Request.Path.ToString().ToLower(),
                    contextAccessor.HttpContext.Request.QueryString.HasValue ? contextAccessor.HttpContext.Request.QueryString.Value : null,
                    request,
                    Log.Categoria.Geral.ToInt(),
                    Log.SubCategoria.Entrada.ToInt(),
                    contextAccessor.HttpContext.TraceIdentifier,
                    contextAccessor.HttpContext.Request.Method,
                    contextAccessor.HttpContext.Connection.RemoteIpAddress,
                    startTime,
                    null,
                    userId);
            }

            var originalBodyStream = context.Response.Body;

            /// var responseBody = new MemoryStream();
            try
            {
                // context.Response.Body = responseBody;

                await _next(context);

                // var response = await FormatResponse(context.Response);

                // Não gravar o payload

                var response = default(string);

                if (gerarLog)
                {
                    log.Logger.Add(
                        Log.Modulo.WebApi.Description(),
                        context.Request.Path.ToString().ToLower(),
                        context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null,
                        response,
                        Log.Categoria.Geral.ToInt(),
                        Log.SubCategoria.Saida.ToInt(),
                        context.TraceIdentifier,
                        context.Request.Method,
                        context.Connection.RemoteIpAddress,
                        startTime,
                        context.Response.StatusCode,
                        userId);
                }

                // await responseBody.CopyToAsync(originalBodyStream);

            }
            finally
            {
                // context.Response.Body = originalBodyStream;
            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            var bodyAsText = "";

            //try
            //{
            await request.Body.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
            bodyAsText = Encoding.UTF8.GetString(buffer);
            //}
            //catch(Exception e)
            //{
            //    bodyAsText = $"Não foi possível ler a requisição: \n\n{e.Message}";
            //}

            request.Body.Position = 0;

            return bodyAsText;
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            //We need to read the response stream from the beginning...
            response.Body.Seek(0, SeekOrigin.Begin);

            //...and copy it into a string
            string text = await new StreamReader(response.Body).ReadToEndAsync();

            //We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);

            //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
            return text;
        }

        public bool GerarLog(string module, string action)
        {
            module = ((module.Substring(module.Length - 1) == "/") ?
                module.Substring(0, module.Length - 1) : module).ToLower();
            action = action.ToLower();
            var result = !ModulosSemLog().Where(i => IsMatch(i, module, action)).Any();
            return result;
        }

        private bool IsMatch(ModuloSemLog i, string module, string action)
        {
            var result = false;
            if ((i.Action == action) || (i.Action == "*"))
            {
                if ((i.ModuleName == module))
                {
                    result = true;
                }
                else
                if (
                    (i.ModuleName.EndsWith('*')) &&
                    (module.StartsWith(i.ModuleName.Substring(0, i.ModuleName.Length - 1)))
                   )
                {
                    result = true;
                }
            }
            return result;
        }

        private IEnumerable<ModuloSemLog> ModulosSemLog()
        {
            if (_moduloSemLog == null)
            {
                _moduloSemLog = new List<ModuloSemLog>
                {
                    /*
                    new ModuloSemLog("/file", "*"),
                    new ModuloSemLog("/file/*", "*"),
                    new ModuloSemLog("/downloadfile", "*"),
                    new ModuloSemLog("/downloadfile/*", "*"),
                    new ModuloSemLog("/defaulthub/*", "*"),
                    new ModuloSemLog("/log", "*"),
                    new ModuloSemLog("/log/*", "*"),
                    new ModuloSemLog("/mapa/monitoramento/resumo*", "*"),
                    new ModuloSemLog("/maintenance*", "*"),
                    new ModuloSemLog("/h1*", "*"),
                    new ModuloSemLog("/notificacaodealteracaodelocal*", "*"),
                    */
                     
                    new ModuloSemLog("/index.html", "*"),
                    new ModuloSemLog("", "*"),
                    new ModuloSemLog("/favicon.ico", "*"),
                    new ModuloSemLog("/publickey*", "*"),
                    new ModuloSemLog("/log", "*"),
                    new ModuloSemLog("/mapa/monitoramento/resumo*", "*"),
                    new ModuloSemLog("/file", "*"),
                    new ModuloSemLog("/file/*", "*"),
                    new ModuloSemLog("/downloadfile", "*"),
                    new ModuloSemLog("/downloadfile/*", "*"),
                    new ModuloSemLog("/equipamento/dadosdemonitoramentodetemperatura", "*"),
                    new ModuloSemLog("/equipamento/historicodetemperatura", "*"),
                    new ModuloSemLog("/equipamento/dadosdemonitoramentodeumidade", "*"),
                    new ModuloSemLog("/equipamento/historicodeumidade", "*"),
                    new ModuloSemLog("/evento/quantidadedeeventospendentes", "*"),
                    new ModuloSemLog("/monitoramentoemtemporeal/historico", "*"),
                    new ModuloSemLog("/monitoramentoemtemporeal/dadosdemonitoramento", "*"),
                };
            }
            return _moduloSemLog;
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class RequestResponseLoggingExtensions
    {
        public static IApplicationBuilder UseRequestResponseLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }

    internal class ModuloSemLog
    {
        public string ModuleName { get; set; }
        public string Action { get; set; }

        public ModuloSemLog(string moduleName, string action)
        {
            this.ModuleName = moduleName;
            this.Action = action;
        }
    }
}
