using api.BackgroundServices;
using api.Middlewares;
using framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private static string[] allowedOrigins;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            SetConfigurations(configuration);
        }

        private void SetConfigurations(IConfiguration configuration)
        {
            Cfg.FileStoragePath = Configuration.GetValue<string>("FileStoragePath");
            Cfg.RSAPrivateKeyEncrypted = Configuration.GetValue<string>("RSAPrivateKeyEncrypted");
            Cfg.RSAPublicKey = Configuration.GetValue<string>("RSAPublicKey");
            Cfg.RunServices = Configuration.GetValue<string>("RunServices") == "True";
            Cfg.LogPackages = Configuration.GetValue<string>("LogPackages") == "True";
            Cfg.PublicApiUrl = Configuration.GetValue<string>("PublicApiUrl");
            Cfg.PdfBuilderUrl = Configuration.GetValue<string>("PdfBuilderUrl");
            Cfg.RecaptchaKey = "6LfAororAAAAACF2b6cgtXBEvDfb3JSmO4aG1-ND";
            allowedOrigins = Configuration.GetSection("AllowedOrigins").Get<string[]>();

            Cfg.Validate();
            EncryptedText.SetKey(new WebApiKeyProvider());
            log.Logger.SetupLog(api.Log.Builders.LogSetupBuilder.Build(configuration, new WebApiKeyProvider()));
        }

        // ========= 🔹 Configuração de Serviços =========
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.ContractResolver =
                        new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // ======== 🔹 CORS ========
            services.AddCors(options =>
            {
                if (allowedOrigins != null && allowedOrigins.Length > 0)
                {
                    options.AddPolicy("ConfiguredOrigins", builder =>
                    {
                        builder.WithOrigins(allowedOrigins)
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
                }
                else
                {
                    options.AddPolicy("AllowAll", builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
                }
            });

            ConfigureDefaultServices(services);
            ConfigureBackgroundServices(services);
        }

        public void ConfigureDefaultServices(IServiceCollection services)
        {
            Dependencies.ConfigureApplicationDependencies(services, Configuration);
        }

        public void ConfigureBackgroundServices(IServiceCollection services)
        {
            if (Configuration.GetValue<string>("RunServices") == "True")
            {
                Services.Install(services);
            }
        }

        // ========= 🔹 Configuração do pipeline =========
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // 🔹 Aplica a policy correta
            app.UseCors(allowedOrigins != null && allowedOrigins.Length > 0
                ? "ConfiguredOrigins"
                : "AllowAll");

            // Middlewares
            app.UseErrorHandlingMiddleware();
            app.UseRequestResponseLoggingMiddleware();
            app.UseRequestStartupMiddleware();
            app.UseAuthenticationMidlleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            using (var scope = app.ApplicationServices.CreateScope())
            {
                AppInit.Init(scope.ServiceProvider);
            }
        }
    }
}