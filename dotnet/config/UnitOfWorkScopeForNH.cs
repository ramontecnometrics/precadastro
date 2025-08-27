using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using data;
using NHibernate;
using NHibernate.SqlCommand;
using System;
using System.Runtime.CompilerServices;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

[assembly: InternalsVisibleTo("setup, PublicKey=0024000004800000940000000602000000240000525341310004000001000100916d2a8526e75ce7a6e4cf7574b80d4f793339e44c64c7eedf425df6e188e1916a6f0647f1eb5ee7e6fbf348dc0d55b635b87f13605678c685c5f13084a8bfc5fc16e94b036f090f269573130a9c404f0c7df8bbf44d4e67f594a12d506e5eef4562f063c274ac0d7885c4296db28136ad18a5bbf81f2c0a52dd628711e044d3")]

namespace config
{
    public class UnitOfWorkScopeForNH : UnitOfWorkScope
    {
        private ISession _session;
        private Object _lock = new Object();

        public UnitOfWorkScopeForNH(IUnitOfWorkScopeParams unitOfWorkScopeParams) :
            base(unitOfWorkScopeParams)
        {

        }

        public override void StartTransaction()
        {
            GetSession().BeginTransaction();
        }

        public override void Commit()
        {
            _session.Flush();
            _session.GetCurrentTransaction().Commit();
        }

        public override void Rollback()
        {
            _session.GetCurrentTransaction().Rollback();
        }

        public override void CloseSession()
        {
            lock (_lock)
            {
                if (_session != null)
                {
                    /*if (_session.IsConnected)
                    {
                        _session.Close();
                    }*/
                    _session.Dispose();
                    _session = null;
                }
            }
        }

        private ISession GetSession()
        {
            lock (_lock)
            {
                if (_session == null)
                {
                    _session = NHSessionFactory.OneISessionFactory(_connectionString).OpenSession();
                }
                if (!_session.IsConnected)
                {
                    throw new Exception("Transaction was not connected.");
                }
                return _session;
            }
        }

        public ISession Session()
        {
            return GetSession();
        }
    }

    internal static class NHSessionFactory
    {
        private static ISessionFactory _oneISessionFactory;

        internal static bool recreateDatabaseSchema = false;
        internal static bool updateDatabaseSchema = false;

        internal static ISessionFactory OneISessionFactory(string connectionString)
        {
            if (_oneISessionFactory == null)
            {
                var configMap = GetConfigMap(connectionString);
                _oneISessionFactory = configMap.BuildSessionFactory();
            }
            return _oneISessionFactory;
        }

        internal static FluentConfiguration GetConfigMap(string connectionString)
        {
            var configurer = PostgreSQLConfiguration.PostgreSQL82.ConnectionString(connectionString);
            var configMap = Fluently.Configure()
                .Database(configurer)
                .ExposeConfiguration(c => c.SetProperty(global::NHibernate.Cfg.Environment.ReleaseConnections, "on_close"))
                .Mappings(m =>
                {
                    m.FluentMappings.AddFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
                    m.FluentMappings.Conventions.Setup(s => s.Add(AutoImport.Never()));
                    m.FluentMappings.Conventions.Add(ConventionBuilder.Class.Always(x => x.Table(x.EntityType.Name)));
                    m.FluentMappings.Conventions.Add(new JoinedSubclassConvention());

                    m.FluentMappings.Conventions.Add(FluentNHibernate.Conventions.Helpers.DefaultLazy.Never());
                    m.FluentMappings.Conventions.Add(FluentNHibernate.Conventions.Helpers.ConventionBuilder.Class.Always(x => x.Not.LazyLoad()));
                    m.FluentMappings.Conventions.Add(ConventionBuilder.Reference.Always(x => x.Index(string.Format("idx_{0}_{1}", x.EntityType.Name, x.Name))));
                    m.FluentMappings.Conventions.Add(ConventionBuilder.Reference.Always(x => x.ForeignKey(string.Format("fk_{0}_{1}", x.EntityType.Name, x.Name))));

                    m.FluentMappings.Conventions.Add(ConventionBuilder.Reference.Always(x => x.Cascade.None()));
                    m.FluentMappings.Conventions.Add(ConventionBuilder.HasMany.Always(x => x.Cascade.None()));
                    m.FluentMappings.Conventions.Add(ConventionBuilder.HasMany.Always(x => x.Key.ForeignKey("none")));

                    m.FluentMappings.Conventions.Add(
                        ConventionBuilder.Property.When(
                            c => c.Expect(p => p.Property.PropertyType == typeof(string)),
                            i => i.CustomType("AnsiString")
                        )
                    );

                    m.FluentMappings.Conventions.Add(
                        ConventionBuilder.Property.When(
                            c => c.Expect(p => p.Property.PropertyType == typeof(framework.EncryptedText)),
                            i => i.CustomType<NHibernate.EncryptedTextType>())
                    );
                });

            var arquivo = System.IO.Path.Combine(AppContext.BaseDirectory, "appsettings.json");

            if (System.IO.File.Exists(System.IO.Path.Combine(AppContext.BaseDirectory, "appsettings.json"))){
                var settings = Newtonsoft.Json.Linq.JObject.Parse(
                    System.IO.File.ReadAllText(arquivo));

                if (settings["LogSql"] != null && settings["LogSql"].ToString() == "True")
                {
                    configMap.ExposeConfiguration(c => c.SetInterceptor(new SqlStatementInterceptor()));
                }
            }

            return configMap;
        }
    }

    internal class SqlStatementInterceptor : EmptyInterceptor
    {
        const string linha = "============================================================================================================================================================";
        public override SqlString OnPrepareStatement(SqlString sql)
        {
            var sqlp = base.OnPrepareStatement(sql);
#if DEBUG
            Console.WriteLine(linha);
            Console.WriteLine(sqlp);
            Console.WriteLine(linha);
#endif
            return sqlp;
        }
    }

    public class JoinedSubclassConvention : IJoinedSubclassConvention
    {
        public void Apply(IJoinedSubclassInstance instance)
        {
            instance.Table(instance.EntityType.Name);
            instance.Key.Column("id");
        }
    }
}
