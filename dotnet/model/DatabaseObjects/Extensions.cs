namespace model.DatabaseObjects
{
    public class Extensions
    {
        public static string GetSql()
        {
            var result = @"
CREATE EXTENSION IF NOT EXISTS pg_stat_statements;
CREATE EXTENSION IF NOT EXISTS dblink;
";
            return result;
        }
    }
}
