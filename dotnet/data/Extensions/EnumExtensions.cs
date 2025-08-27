using System;
using System.Linq.Expressions;

namespace data.Extensions
{
    public class EnumExtensions
    {
        private static string GetDatabaseCheckConstraint<T>(string column)
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

        public static string GetDatabaseCheckConstraint<TClass, TProperty>(Expression<Func<TClass, object>> expression) where TProperty : struct
        {
            var memberExpression = expression.Body as MemberExpression;
            var column = memberExpression.Member.Name;
            var result = GetDatabaseCheckConstraint<TProperty>(column);
            return result;
        }   
    }

}
