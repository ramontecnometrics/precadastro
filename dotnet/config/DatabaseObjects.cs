using NHibernate;
using System;
using System.Collections.Generic;

namespace config
{
    public static class DatabaseObjects
    {
        private static readonly List<KeyValuePair<string, bool>> _scripts = new List<KeyValuePair<string, bool>>();

        internal static void RegisterScript(string script, bool ignoreError = false)
        {
            _scripts.Add(new KeyValuePair<string, bool>(script, ignoreError));
        }

        public static void Execute(ISession session)
        {
            RegisterDatabaseObjects();
            foreach (var script in _scripts)
            {
                Console.WriteLine();
                Console.WriteLine(script);
                var command = session.Connection.CreateCommand();
                command.CommandText = script.Key;
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (System.Exception)
                {
                    if (!script.Value)
                    {
                        throw;
                    }
                }
                Console.WriteLine();
            }
        }

        public static void RegisterDatabaseObjects()
        {
            _scripts.Clear();
            RegisterScript(model.DatabaseObjects.Extensions.GetSql(), false);           
            RegisterScript(model.DatabaseObjects.Grant.GetSql(), true);
        }
    }
}
