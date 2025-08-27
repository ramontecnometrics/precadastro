using config;
using data;
using framework;
using framework.Extensions;
using framework.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using model.Repositories;
using System;
using System.Reflection;
using System.Text;

namespace api
{
    public class Dependencies
    {
        public static void ConfigureApplicationDependencies(IServiceCollection services, IConfiguration configuration)
        {
            AppDependencyRegister.Register(typeof(UnitOfWorkScope), typeof(UnitOfWorkScopeForNH), true,
            (dependency, implementation) =>
            {
                services.AddScoped(dependency, implementation);
            });


            AppDependencyRegister.Register(typeof(IHttpContextAccessor), typeof(HttpContextAccessor), true,
            (dependency, implementation) =>
            {
                services.AddSingleton(dependency, implementation);
            });

            AppDependencyRegister.Register(typeof(WebApiOperationResolver), typeof(WebApiOperationResolver), true,
            (dependency, implementation) =>
            {
                services.AddScoped(dependency, implementation);
            });

            AppDependencyRegister.Register(typeof(IOperationResolver), typeof(WebApiOperationResolver), true,
            (dependency, implementation) =>
            {
                services.AddScoped(dependency, implementation);
            });

            AppDependencyRegister.Register(typeof(Cfg), typeof(Cfg), true,
            (dependency, implementation) =>
            {
                services.AddScoped(dependency, implementation);
            });

            AppDependencyRegister.Register(typeof(IAppContext), typeof(WebApiAppContext), true,
            (dependency, implementation) =>
            {
                services.AddScoped(dependency, (resolver) =>
                {
                    // Obtem o usuário logado a partir do contexto Http.
                    // O HttpContext.User foi atribuído com a classe própria UserClaimsPrincipal 
                    // no midleware de autenticação.
                    var httpContextAccessor = resolver.GetRequiredService<IHttpContextAccessor>();
                    var usuarioLogado = default(model.UsuarioLogado);

                    if (httpContextAccessor.HttpContext != null)
                    {
                        var user = httpContextAccessor.HttpContext.User as UserClaimsPrincipal;
                        if (user != null && user.Id.HasValue)
                        {
                            var respository = resolver.GetRequiredService<Repository<model.UsuarioLogado>>();
                            usuarioLogado = respository.Get(user.Id.Value);
                        }
                    }

                    // Cria uma classe de contexto geral para uso da aplicação fazendo as implementações das
                    // tecnologias necessárias para suprir as dependências da aplicação.
                    var result = new WebApiAppContext()
                    {
                        KeyProvider = resolver.GetRequiredService<IKeyProvider>(),                        
                        User = new WebApiUser(usuarioLogado),
                        PublicApiUrl = Cfg.PublicApiUrl,
                        AuditoriaRepository = resolver.GetRequiredService<AuditoriaRepository>(),
                    };
                    return result;
                });
            });

            services.AddSingleton<IUnitOfWorkScopeParams>((provider) =>
            {
                var keyProvider = new WebApiKeyProvider();

                var connectionString = configuration.GetConnectionString("default");
                var databasePassword = configuration.GetValue<string>("DatabasePassword");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new System.Exception(
                        FormattedString.Build(
                            "A string de conexão com o banco de dados não foi definida.",
                            "\n",
                            "Informe no parâmetro \"ConnectionStrings >> default\" no arquivo appsettings.json."));
                }

                if (connectionString.Contains("Password"))
                {
                    throw new System.Exception(
                        FormattedString.Build(
                            "A string de conexão com o banco de dados não pode ter a propriedade \"Password\".",
                            "\n",
                            "Verifique o parâmetro \"ConnectionStrings >> default\" no arquivo appsettings.json."));
                }

                if (string.IsNullOrEmpty(databasePassword))
                {
                    throw new System.Exception(
                        FormattedString.Build(
                            "A senha de conexão com o banco de dados não foi definida.",
                            "\n",
                            "Informe no parâmetro \"DatabasePassword\" no arquivo appsettings.json."));
                }

                if (databasePassword.Length % 4 != 0)
                {
                    throw new System.Exception(
                        FormattedString.Build(
                            "A senha de conexão com o banco de dados deve estar criptografada e no formato base64.",
                            "\n",
                            "Verifique o parâmetro \"DatabasePassword\" no arquivo appsettings.json."));
                }

                var databasePasswordDecryted = default(string);

                try
                {
                    databasePasswordDecryted = SymmetricEncryption.DecryptString(databasePassword,
                        UTF8Encoding.UTF8.GetBytes(keyProvider.GetKey().ToPlainText()),
                        UTF8Encoding.UTF8.GetBytes(keyProvider.GetIV().ToPlainText())
                    );
                }
                catch (Exception e)
                {
                    /* Tem um projeto de testes para criptografar textos.*/
                    throw new Exception(
                        FormattedString.Build("Não foi possível descriptografar a string de conexão com o banco de dados.",
                            "\n",
                            "Verifique o parâmetro \"DatabasePassword\" no arquivo appsettings.json."),
                            e
                        );
                }

                var result = new UnitOfWorkScopeParams()
                {
                    ConnectionString = string.Format(
                        "{0};Password={1}",
                        connectionString,
                        databasePasswordDecryted),
                    DefaultAction = UnitOfWorkScopeDefaultAction.Rollback,
                    UseTransaction = true,
                };
                return result;
            });

            AppDependencyRegister.Register(typeof(IUserValidator), typeof(WebApiUserValidator), true,
            (dependency, implementation) =>
            {
                services.AddTransient(dependency, implementation);
            });

            AppDependencyRegister.Register(typeof(IKeyProvider), typeof(WebApiKeyProvider), true,
            (dependency, implementation) =>
            {
                services.AddTransient(dependency, implementation);
            });

            AppDependencyRegister.Register(typeof(AccessControl), typeof(AccessControl), true,
            (dependency, implementation) =>
            {
                services.AddTransient(dependency, implementation);
            });

            AppDependencyRegister.Register(typeof(IUnitOfWork<>), typeof(UnitOfWorkForHN<>), true,
            (dependency, implementation) =>
            {
                services.AddTransient(dependency, implementation);
            });

            AppDependencyRegister.Register(typeof(FileManager), typeof(FileManager), true,
            (dependency, implementation) =>
            {
                services.AddTransient(dependency, implementation);
            });

            AppDependencyRegister.Register(typeof(ISqlCommand), typeof(NHCommand), true,
            (dependency, implementation) =>
            {
                services.AddTransient(dependency, implementation);
            });

            AppDependencyRegister.Register(typeof(model.Pessoa).Assembly, true,
            (dependency, implementation) =>
            {
                services.AddTransient(dependency, implementation);
            });


            AppDependencyRegister.Register(typeof(data.Repository<>).Assembly, true,
            (dependency, implementation) =>
            {
                services.AddTransient(dependency, implementation);
            });

            AppDependencyRegister.Register(Assembly.GetExecutingAssembly(), "api.Controllers", true,
            (dependency, implementation) =>
            {
                services.AddTransient(dependency, implementation);
            });

            AppDependencyRegister.Register(Assembly.GetExecutingAssembly(), "api.Conversors", true,
            (dependency, implementation) =>
            {
                services.AddTransient(dependency, implementation);
            });
        }
    }
}