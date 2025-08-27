using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;

namespace config
{
    public static class Views
    {
        private static readonly List<KeyValuePair<string, bool>> _scripts = new List<KeyValuePair<string, bool>>();

        internal static void RegisterViewScript(string script, bool ignoreError = false)
        {
            _scripts.Add(new KeyValuePair<string, bool>(script, ignoreError));
        }

        public static void Execute(ISession session)
        {
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
    }
}
