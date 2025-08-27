using framework.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NHibernate.Engine;
using System;
using System.IO;
using System.Text;

namespace api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // ========= 🔑 Chave/IV para descriptografar senha do certificado =========
            var keyProvider = new WebApiKeyProvider();
            var key = Encoding.UTF8.GetBytes(keyProvider.GetKey().ToPlainText());
            var iv = Encoding.UTF8.GetBytes(keyProvider.GetIV().ToPlainText());

            // ========= ⚙️ Configurações =========
            var baseDirectory = AppContext.BaseDirectory;
            var cfg = new ConfigurationBuilder()
                .SetBasePath(baseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Cfg.LogSql = cfg["LogSql"] == "True";
            var runApi = cfg["RunApi"] == "True";
            var runServices = cfg["RunServices"] == "True";

            var port = string.IsNullOrWhiteSpace(cfg["HttpPort"]) ? "80" : cfg["HttpPort"].Trim();
            var sslCertificateFile = cfg["SslCertificateFile"];
            var sslCertificatePassword = cfg["SslCertificatePassword"];

            // ========= 🌐 Definição da URL =========
            var useHttps = !string.IsNullOrEmpty(sslCertificateFile);
            var url = $"{(useHttps ? "https" : "http")}://*:{port}";
            Cfg.Https = useHttps;

            // ========= 🕒 Compatibilidade Npgsql =========
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            // ========= 🚀 Execução =========
            if (runApi)
            {
                RunApi(args, baseDirectory, url, port, sslCertificateFile, sslCertificatePassword, key, iv, useHttps);
            }
            else if (runServices)
            {
                RunServices(args, baseDirectory);
            }
            else
            {
                Console.WriteLine("⚠ Nenhum modo selecionado (RunApi ou RunServices). Verifique appsettings.json.");
            }
        }

        private static void RunApi(
            string[] args,
            string baseDirectory,
            string url,
            string port,
            string sslCertificateFile,
            string sslCertificatePassword,
            byte[] key,
            byte[] iv,
            bool useHttps)
        {
            try
            {
                var hostBuilder = WebHost.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config.SetBasePath(baseDirectory)
                              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    })
                    .ConfigureKestrel(options =>
                    {
                        options.Limits.MinRequestBodyDataRate = null;

                        if (useHttps)
                        {
                            if (!File.Exists(sslCertificateFile))
                            {
                                throw new FileNotFoundException($"Arquivo de certificado não encontrado: {sslCertificateFile}");
                            }

                            var certPassword = framework.Security.SymmetricEncryption.DecryptString(sslCertificatePassword, key, iv);

                            options.ListenAnyIP(int.Parse(port), listenOptions =>
                            {
                                listenOptions.UseHttps(sslCertificateFile, certPassword);
                            });
                        }
                        else
                        {
                            options.ListenAnyIP(int.Parse(port));
                        }
                    });

                // Só usa UseUrls se não houver SSL
                if (!useHttps)
                {
                    hostBuilder.UseUrls(url);
                }

                hostBuilder
                    .UseStartup<Startup>()
                    .Build()
                    .Run();
            }
            catch (Exception e)
            {
                PrintError(e);
            }
        }

        private static void RunServices(string[] args, string baseDirectory)
        {
            try
            {
                var hostBuilder = Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config.SetBasePath(baseDirectory)
                              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    })
                    .ConfigureServices((ctx, services) =>
                    {
                        var configuration = ctx.Configuration;
                        var startup = new Startup(configuration);
                        startup.ConfigureDefaultServices(services);
                        startup.ConfigureBackgroundServices(services);
                    });

                Console.WriteLine("Iniciando em modo de serviço...");
                var host = hostBuilder.Build();
                AppInit.Init(host.Services);
                host.Run();
            }
            catch (Exception e)
            {
                PrintError(e);
            }
        }

        private static void PrintError(Exception e)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e);
            Console.ForegroundColor = color;
        }
    }
}
