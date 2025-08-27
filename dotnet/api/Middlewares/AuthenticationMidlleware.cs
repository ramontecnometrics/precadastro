using framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace api
{
    public class AuthenticationMidlleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMidlleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, api.Cfg cfg)
        {
            if (httpContext.Request.Method != "OPTIONS")
            {
                string httpUrl = httpContext.Request.Path;
                string httpMethod = httpContext.Request.Method.ToUpper();

                long? userId = null;
                long? representingId = null;
                string authorization;

                // Tratamento específico para o Hub do SignalR
                if ((httpUrl != null) &&
                    (httpUrl.StartsWith("/defaulthub")))
                {
                    string authorizationHeader = httpContext.Request.Headers["Authorization"];
                    string bearerAuthorization = null;

                    // Caso autenticação seja informada no Header
                    if (authorizationHeader.Contains(":"))
                    {
                        bearerAuthorization = authorizationHeader;
                    }
                    else
                    if (httpContext.Request.Query.ContainsKey("authorization"))
                    {
                        bearerAuthorization = httpContext.Request.Query["authorization"];
                    }

                    if (string.IsNullOrEmpty(bearerAuthorization))
                    {
                        throw new System.Exception("Missing authorization info.");
                    }

                    if (bearerAuthorization.Contains("Bearer "))
                    {
                        throw new System.Exception("Authorization is not in bearer format.");
                    }

                    if (bearerAuthorization.Contains(":"))
                    {
                        throw new System.Exception("Authorization does not contains user ID. Send the authorization in the format \"Bearer <UserID>:<Authorization>\".");
                    }

                    var strArray = bearerAuthorization.Split(":");
                    var userIdString = strArray[0].Replace("Bearer ", "");

                    if (long.TryParse(userIdString, out long parsedUserId))
                    {
                        userId = parsedUserId;
                    }

                    if ((!string.IsNullOrEmpty(userIdString) && (!userId.HasValue)))
                    {
                        throw new System.Exception("User ID sent in authorization is not a valid integer.");
                    }

                    authorization = strArray[1];
                }
                else
                if (IsJwt(httpContext.Request.Headers["Authorization"]))
                {
                    authorization = httpContext.Request.Headers["Authorization"];                    
                }
                else
                {
                    authorization = httpContext.Request.Headers["Authorization"];
                    string userIdString = httpContext.Request.Headers["UserId"];
                    string RepresentingIdString = httpContext.Request.Headers["RepresentingId"];

                    if (long.TryParse(userIdString, out long parsedUserId))
                    {
                        userId = parsedUserId;
                    }

                    if (((!string.IsNullOrEmpty(userIdString)) && (userIdString != "null")) && (!userId.HasValue))
                    {
                        throw new System.Exception("User ID sent in authorization is not a valid integer.");
                    }

                    if (long.TryParse(RepresentingIdString, out long parsedRepresentingId))
                    {
                        representingId = parsedRepresentingId;
                    }
                }

                // Validar o acesso do usuário
                var operation = cfg.OperationResolver.Resolve(httpMethod, httpUrl);
                var user = cfg.AccessControl.ValidadeCredentials(userId, authorization, operation);
                user.RepresentingId = representingId;

                var authenticationStatus = user.Authentication.Status;
                if (!((authenticationStatus == AuthenticationStatus.Authenticated) ||
                     (authenticationStatus == AuthenticationStatus.AuthenticationNotRequired)))
                {
                    PrepareResponse(httpContext);
                    httpContext.Response.StatusCode = 401; //UnAuthorized   
                    await httpContext.Response.WriteAsync(GetMessage(authenticationStatus));
                    return;
                }

                httpContext.User = user.ToClaimsPrincipal();

                var accessValidationStatus = cfg.AccessControl.CanExecuteOperation(user, operation);

                if (accessValidationStatus != AccessValidationStatus.AccessGranted)
                {
                    PrepareResponse(httpContext);
                    httpContext.Response.StatusCode = 403; //Access Denied   
                    var errorMessage = "";

                    switch (accessValidationStatus)
                    {
                        case AccessValidationStatus.AccessDenied:
                            errorMessage = 
                                 operation == null ?
                              "Acesso negado à operação." :
                                 $"Acesso negado à operação: {operation.Id} - {operation.Description}";
                            break;

                        case AccessValidationStatus.BlockedContract:
                            errorMessage = "Este serviço foi bloqueado devido a restrições contratuais. Caso tenha dúvidas, procure o suporte.";
                            break;

                        case AccessValidationStatus.FinishedContract:
                            errorMessage = "Este serviço não está mais disponível pois o contrato foi encerrado. Para mais informações, consulte o suporte.";
                            break;
                        default: break;
                    }

                    await httpContext.Response.WriteAsync(errorMessage);
                    return;
                }

                if (representingId.HasValue)
                {
                    // Se estiver usando representação repete as validações porém agora para o usuário sendo representado
 
                    var representedUser = new User()
                    {
                        Id = user.Id.Value,
                        Name = string.Format("ID {0}", representingId.Value),
                        AuthenticationType = string.Format("Represented by ID {0} [{1}]", user.Id.Value, user.Name),
                        IsAuthenticated = true,
                        Authentication = new Authentication()
                        {
                            Date = user.Authentication.Date,
                            Status = user.Authentication.Status,
                            RequestToken = user.Authentication.RequestToken,
                            TokenUID = user.Authentication.TokenUID
                        },
                    };

                    authenticationStatus = representedUser.Authentication.Status;
                    if (!((authenticationStatus == AuthenticationStatus.Authenticated) ||
                         (authenticationStatus == AuthenticationStatus.AuthenticationNotRequired)))
                    {
                        PrepareResponse(httpContext);
                        httpContext.Response.StatusCode = 401; //UnAuthorized   
                        await httpContext.Response.WriteAsync(GetMessage(authenticationStatus));
                        return;
                    }
                }

            }
            await _next.Invoke(httpContext);
        }

        private bool IsJwt(StringValues stringValues)
        {
            var result = false;
            string value = stringValues;

            if (!string.IsNullOrEmpty(value) && value.Split(".").Length == 3)
            {
                result = true;
            }

            return result;
        }

        private string GetMessage(AuthenticationStatus authenticationStatus)
        {
            var dict = new Dictionary<AuthenticationStatus, string>
            {
                { AuthenticationStatus.Authenticated, "Autenticado." },
                { AuthenticationStatus.AuthenticationNotRequired, "Autenticação não necessária." },
                { AuthenticationStatus.InvalidUser, "Usuário inválido." }
            };
            return dict[authenticationStatus];
        }

        private void PrepareResponse(HttpContext httpContext)
        {
            httpContext.Response.Headers[HeaderNames.AccessControlAllowOrigin] = httpContext.Request.Headers[HeaderNames.Origin];
            //httpContext.Response.Headers[HeaderNames.AccessControlAllowOrigin] = "null";
            httpContext.Response.Headers[HeaderNames.AccessControlAllowCredentials] = "true";
            httpContext.Response.Headers[HeaderNames.AccessControlAllowHeaders] = "X-Requested-With, Content-Type, Accept, UserId, RepresentingId, Authorization, lang";
            httpContext.Response.Headers[HeaderNames.AccessControlAllowMethods] = "OPTIONS, GET, POST, PUT, DELETE";
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AuthenticationMidllewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationMidlleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMidlleware>();
        }
    }
}
