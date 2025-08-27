using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace framework.Extensions
{
    public static class IQueryableExtensions
    {
        public static IOrderedQueryable<T> Order<T>(this IQueryable<T> query, string propertyNames)
        {
            IOrderedQueryable<T> result = null;
            if (!string.IsNullOrWhiteSpace(propertyNames))
            {
                var arr = propertyNames.Split(',');
                var primeiro = true;
                foreach (var i in arr)
                {
                    var propName = i.Trim();
                    var arr2 = propName.Split(' ');
                    propName = arr2[0];
                    var ascDesc = "asc";
                    if (arr2.Count() > 1)
                    {
                        ascDesc = arr2[1].Trim().ToLowerInvariant();
                    }

                    if (primeiro)
                    {
                        if (ascDesc == "desc")
                        {
                            result = OrderByDescending(query, propName);
                        }
                        else
                        {
                            result = OrderBy(query, propName);
                        }
                    }
                    else
                    {
                        if (ascDesc == "desc")
                        {
                            result = result.ThenByDescending(propName);
                        }
                        else
                        {
                            result = result.ThenBy(propName);
                        }
                    }
                    primeiro = false;

                }
            }
            else
            {
                result = OrderBy(query, "Id");
            }
            return result;
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName, IComparer<object> comparer = null)
        {
            return CallOrderedQueryable(query, "OrderBy", propertyName, comparer);
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string propertyName, IComparer<object> comparer = null)
        {
            return CallOrderedQueryable(query, "OrderByDescending", propertyName, comparer);
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> query, string propertyName, IComparer<object> comparer = null)
        {
            return CallOrderedQueryable(query, "ThenBy", propertyName, comparer);
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> query, string propertyName, IComparer<object> comparer = null)
        {
            return CallOrderedQueryable(query, "ThenByDescending", propertyName, comparer);
        }

        public static IOrderedQueryable<T> CallOrderedQueryable<T>(this IQueryable<T> query, string methodName, string propertyName,
            IComparer<object> comparer = null)
        {
            if (typeof(T) != typeof(Object))
            {
                var param = Expression.Parameter(typeof(T), "x");

                var body = propertyName.Split('.').Aggregate<string, Expression>(param, Expression.PropertyOrField);

                return comparer != null
                    ? (IOrderedQueryable<T>)query.Provider.CreateQuery(
                        Expression.Call(
                            typeof(Queryable),
                            methodName,
                            new[] { typeof(T), body.Type },
                            query.Expression,
                            Expression.Lambda(body, param),
                            Expression.Constant(comparer)
                        )
                    )
                    : (IOrderedQueryable<T>)query.Provider.CreateQuery(
                        Expression.Call(
                            typeof(Queryable),
                            methodName,
                            new[] { typeof(T), body.Type },
                            query.Expression,
                            Expression.Lambda(body, param)
                        )
                    );
            }
            else
            {
                return query.OrderBy(i => 0);
            }
        }
    }
}
