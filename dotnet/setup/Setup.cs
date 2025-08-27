using NHibernate.Tool.hbm2ddl;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace setup
{
    public class Setup : CustomProgram
    {
        private static readonly byte[] _usuario_ramon = new byte[] { 114, 97, 109, 111, 110 };
        private static readonly byte[] _senha_ramon = new byte[] { 101, 114, 116, 53, 53, 48 };

        public static void Main(string[] args)
        {
            Execute<Setup>();
        }

        protected override void RegistrarOpcoes()
        {
#if DEBUG
            AdicionarOpcao("criar novo banco de dados", () => CriarBancoDeDados());
            AdicionarOpcao("recriar objetos", () => RecriarObjetosDeBancoDeDados());
            AdicionarOpcao("atualizar objetos", () => AtualizarObjetosDeBancoDeDados());
            AdicionarOpcao("sair", () => Sair());
#else
            AdicionarOpcao("login", () => Logar());
            AdicionarOpcao("sair", () => Sair());
#endif
        }

        protected virtual bool Logar()
        {
            Clear();
            var usuarioInformado = ObterValor<string>("Usuário:");
            var senhaInformada = ObterValor<string>("Senha:");

            var loginOk = LoginOk(usuarioInformado, senhaInformada);

            if (loginOk)
            {
                LimparOpcoes();
                AdicionarOpcao("criar novo banco de dados", () => CriarBancoDeDados());
                AdicionarOpcao("recriar objetos", () => RecriarObjetosDeBancoDeDados());
                AdicionarOpcao("atualizar objetos", () => AtualizarObjetosDeBancoDeDados());
                AdicionarOpcao("sair", () => Sair());
            }
            else
            {
                WriteLine("Usuário ou senha inválida.");
                Esperar();
            }
            return true;
        }

        private bool LoginOk(string usuarioInformado, string senhaInformada)
        {
            var result = false;
            var usuarios = new Dictionary<string, string>
            {
                { UTF8Encoding.UTF8.GetString(_usuario_ramon), UTF8Encoding.UTF8.GetString(_senha_ramon) },
            };

            if (usuarios.ContainsKey(usuarioInformado))
            {
                result = usuarios[usuarioInformado] == senhaInformada;
            }
            return result;
        }

        protected virtual bool AtualizarObjetosDeBancoDeDados()
        {
            Clear();
            var schema = ObterValor<string>("Schema [DEFAULT ou LOG]:", (valor) =>
            {
                valor = string.IsNullOrEmpty(valor) ? "DEFAULT" : valor;
                var valoresValidos = new string[] { "DEFAULT", "LOG", "default", "log" };
                if (!valoresValidos.Contains(valor))
                {
                    throw new System.Exception("Schema inválido. Deve ser \"DEFAULT\" ou \"LOG\".");
                }
            });
            schema = string.IsNullOrEmpty(schema) ? "DEFAULT" : schema;
            var parametrosDeAcessoAoBanco = GetParametrosDeAcessoABanco();
            var confirma = ObterValor<bool>(
                string.Format("Servidor {0}:{1}, banco de dados {2}. Confirma a atualização dos objetos?",
                    parametrosDeAcessoAoBanco.Servidor, parametrosDeAcessoAoBanco.Porta,
                    parametrosDeAcessoAoBanco.NomeDoBancoDeDados));
            if (confirma)
            {
                var connectionString = parametrosDeAcessoAoBanco.GetConnectionString();
                UpdateDatabaseSchema(connectionString, schema);
                WriteLine("Objetos atualizados com sucesso!");
                Esperar();
            }
            return true;
        }

        protected virtual bool RecriarObjetosDeBancoDeDados()
        {
            Clear();
            var schema = ObterValor<string>("Schema [DEFAULT ou LOG]:", (valor) =>
            {
                valor = string.IsNullOrEmpty(valor) ? "DEFAULT" : valor;
                var valoresValidos = new string[] { "DEFAULT", "LOG", "default", "log", null };
                if (!valoresValidos.Contains(valor))
                {
                    throw new System.Exception("Schema inválido. Deve ser \"DEFAULT\" ou \"LOG\".");
                }
            });
            schema = string.IsNullOrEmpty(schema) ? "DEFAULT" : schema;
            var parametrosDeAcessoAoBanco = GetParametrosDeAcessoABanco();
            var confirma = ObterValor<bool>(string.Format(
                @"Atenção!!!
Essa operação vai APAGAR TODOS OS DADOS do banco de dados {0} no servidor {1}:{2}. Deseja realmente prosseguir?",
                 parametrosDeAcessoAoBanco.NomeDoBancoDeDados, parametrosDeAcessoAoBanco.Servidor,
                 parametrosDeAcessoAoBanco.Porta));
            if (confirma)
            {
                var connectionString = parametrosDeAcessoAoBanco.GetConnectionString();
                RecreateDatabaseSchema(connectionString, schema);
                WriteLine("Objetos recriados com sucesso!");
                Esperar();
            }
            return true;
        }

        protected virtual void RecreateDatabaseSchema(string connectionString, string schema)
        {
            FluentNHibernate.Cfg.FluentConfiguration configMap;
            if (schema.ToUpperInvariant() == "DEFAULT")
            {
                configMap = config.NHSessionFactory.GetConfigMap(connectionString);
            }
            else
            if (schema.ToUpperInvariant() == "LOG")
            {
                configMap = log.NHibernate.NHSessionFactory.GetConfigMap(connectionString);
            }
            else
            {
                throw new System.Exception("Schema inválido. Deve ser \"DEFAULT\" ou \"LOG\".");
            }
            configMap.ExposeConfiguration(cfg => new SchemaExport(cfg).Execute(true, true, false));

            if (schema.ToUpperInvariant() == "DEFAULT")
            {
                using (var sessionFactory = configMap.BuildSessionFactory())
                {
                    using (var session = sessionFactory.OpenSession())
                    {
                        config.Views.Execute(session);
                        config.DatabaseObjects.Execute(session);
                    }
                }
            }
            else
            {
                configMap.BuildSessionFactory();
            }
        }

        protected virtual void UpdateDatabaseSchema(string connectionString, string schema)
        {
            FluentNHibernate.Cfg.FluentConfiguration configMap;
            if (schema.ToUpperInvariant() == "DEFAULT")
            {
                configMap = config.NHSessionFactory.GetConfigMap(connectionString);
            }
            else
            if (schema.ToUpperInvariant() == "LOG")
            {
                configMap = log.NHibernate.NHSessionFactory.GetConfigMap(connectionString);
            }
            else
            {
                throw new System.Exception("Schema inválido. Deve ser \"DEFAULT\" ou \"LOG\".");
            }
            configMap.ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true));

            if (schema.ToUpperInvariant() == "DEFAULT")
            {
                using (var sessionFactory = configMap.BuildSessionFactory())
                {
                    using (var session = sessionFactory.OpenSession())
                    {
                        config.Views.Execute(session);
                        config.DatabaseObjects.Execute(session);
                    }
                }
            }
        }

        protected virtual bool CriarBancoDeDados()
        {
            Clear();
            var parametrosDeAcessoAoBanco = GetParametrosDeAcessoABanco();
            var confirma = ObterValor<bool>(string.Format("Servidor {0}:{1}, banco de dados {2}. Confirma a criação?",
                parametrosDeAcessoAoBanco.Servidor, parametrosDeAcessoAoBanco.Porta,
                parametrosDeAcessoAoBanco.NomeDoBancoDeDados));
            var connectionString = parametrosDeAcessoAoBanco.GetConnectionString(false);
            using (var connection = new Npgsql.NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = string.Format(
        @"CREATE DATABASE {0}
    WITH OWNER = {1}
    ENCODING = 'UTF8'
    CONNECTION LIMIT = -1", parametrosDeAcessoAoBanco.NomeDoBancoDeDados, parametrosDeAcessoAoBanco.Usuario);
                command.ExecuteNonQuery();
                WriteLine("Banco de dados criado com sucesso!");
                Esperar();
            }
            return true;
        }

        protected virtual ParametrosDeAcessoABanco GetParametrosDeAcessoABanco()
        {
            var result = new ParametrosDeAcessoABanco
            {
                Servidor = ObterValor<string>("Servidor [localhost]:"),
                Porta = ObterValor<int?>("Porta [5432]:"),
                Usuario = ObterValor<string>("Usuário [postgres]:"),
                Senha = ObterValor<string>("Senha:", (senha =>
                {
                    if (string.IsNullOrWhiteSpace(senha))
                    {
                        throw new System.Exception("Informe a senha de acesso ao banco de dados.");
                    }
                })),
                NomeDoBancoDeDados = ObterValor<string>("Nome do banco de dados:", nome =>
                {
                    if (string.IsNullOrWhiteSpace(nome))
                    {
                        throw new System.Exception("Informe o nome do banco de dados.");
                    }
                    if (nome.ToLower().Trim() == "postgres")
                    {
                        throw new System.Exception("O nome do banco de dados não pode ser postgres.");
                    }
                })
            };

            result.Servidor = !string.IsNullOrWhiteSpace(result.Servidor) ? result.Servidor : "localhost";
            result.Porta = result.Porta.HasValue ? result.Porta.Value : (int?)5432;
            result.Usuario = !string.IsNullOrWhiteSpace(result.Usuario) ? result.Usuario : "postgres";

            return result;
        }
    }
}
