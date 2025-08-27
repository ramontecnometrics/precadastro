using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace log.Data.Extensions
{
    public class Enum<T> where T : struct, IConvertible
    {
        public static string GetDatabaseCheckConstraint(string column)
        {
            var result = string.Empty;
            foreach (var tipo in Enum.GetValues(typeof(T)))
            {
                int i = (int)tipo;
                result += string.Format("({0} = {1}) or ", column, i.ToString());
            }
            result = result.Substring(0, result.Length - 4);
            return result;
        }
    }
}
