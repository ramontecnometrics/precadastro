using framework;
using framework.Extensions;
using Microsoft.Extensions.Configuration;
using log;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using framework.Security;

namespace api.Log.Builders
{
    internal class LogSetupBuilder
    {
        public static LogSetup Build(IConfiguration configuration, IKeyProvider keyProvider)
        {
            var categorias = GetCategorias<Categoria>();
            var subcategorias = GetSubCategorias<SubCategoria>();           

            var connectionString = configuration.GetConnectionString("log");
            var databasePassword = configuration.GetValue<string>("LogDatabasePassword");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new System.Exception(
                    FormattedString.Build(
                        "A string de conexão com o banco de dados de log não foi definida.",
                        "\n",
                        "Informe no parâmetro \"ConnectionStrings >> log\" no arquivo appsettings.json."));
            }

            if (connectionString.Contains("Password"))
            {
                throw new System.Exception(
                    FormattedString.Build(
                        "A string de conexão com o banco de dados de log não pode ter a propriedade \"Password\".",
                        "\n",
                        "Verifique o parâmetro \"ConnectionStrings >> log\" no arquivo appsettings.json."));
            }

            if (string.IsNullOrEmpty(databasePassword))
            {
                throw new System.Exception(
                    FormattedString.Build(
                        "A senha de conexão com o banco de dados de log não foi definida.",
                        "\n",
                        "Informe no parâmetro \"LogDatabasePassword\" no arquivo appsettings.config."));
            }

            if (databasePassword.Length % 4 != 0)
            {
                throw new System.Exception(
                    FormattedString.Build(
                        "A senha de conexão com o banco de dados de log deve estar criptografada e no formato base64.",
                        "\n",
                        "Verifique o parâmetro \"LogDatabasePassword\" no arquivo appsettings.json."));
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
                    FormattedString.Build("Não foi possível descriptografar a string de conexão com o banco de dados de log.",
                        "\n",
                        "Verifique o parâmetro \"LogDatabasePassword\" no arquivo appsettings.json.")
                        ,
                        e
                    );
            }

            var result = new LogSetup()
            {
                Enabled = true,
                ConnectionString = string.Format("{0};Password={1}",connectionString, databasePasswordDecryted),
                AvailableCategories = categorias,
                AvailableSubCategories = subcategorias,
                HostName = Dns.GetHostName(),
            };
            return result;
        }

        public static IEnumerable<Category> GetCategorias<T>()
        {
            var tipos = new List<Category>();
            foreach (var tipo in Enum.GetValues(typeof(T)))
            {
                tipos.Add(new Category()
                {
                    Id = (int)tipo,
                    Description = tipo.Description(),
                });
            }
            return tipos;
        }

        public static IEnumerable<SubCategory> GetSubCategorias<T>()
        {
            var tipos = new List<SubCategory>();
            foreach (var tipo in Enum.GetValues(typeof(T)))
            {
                tipos.Add(new SubCategory()
                {
                    Id = (int)tipo,
                    Description = tipo.Description(),
                    Category = null,
                });
            }
            return tipos;
        }
    }
}
