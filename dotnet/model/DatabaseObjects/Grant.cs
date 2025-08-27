namespace model.DatabaseObjects
{
    public class Grant
    {
        public static string GetSql()
        {
            var result = @"
grant all privileges on database tecnometrics to usertecnometrics;
grant all privileges on all tables in schema public to usertecnometrics;
grant all privileges on all functions in schema public to usertecnometrics;
grant all privileges on all sequences in schema public to usertecnometrics;
";
            return result;
        }
    }
}
