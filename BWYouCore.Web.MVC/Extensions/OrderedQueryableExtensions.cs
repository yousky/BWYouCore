using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace BWYouCore.Web.MVC.Extensions
{
    /// <summary>
    /// IQueryable을 정렬 하여 IOrderedQueryable을 만드는 확장 함수
    /// 
    /// 참조
    /// http://www.codeproject.com/Articles/493917/Dynamic-Querying-with-LINQ-to-Entities-and-Express
    /// http://stackoverflow.com/questions/41244/dynamic-linq-orderby-on-ienumerablet
    /// </summary>
    public static class OrderedQueryableExtensions
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderBy");
        }
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderByDescending");
        }
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenBy");
        }
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenByDescending");
        }
        static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);  //property 대소문자 안 가림
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }


        public static IOrderedQueryable<T> SortBy<T>(this IOrderedQueryable<T> query, string sortInfos)
        {
            string[] sortInfoArray = sortInfos.Split(',');

            var firstPass = true;
            foreach (var sortInfo in sortInfoArray)
            {
                string sortProp = sortInfo;
                bool bDesc = false;
                if (sortInfo.StartsWith("-") == true)
                {
                    bDesc = true;
                    sortProp = sortInfo.Substring(1);
                }

                if (firstPass)
                {
                    firstPass = false;
                    query = bDesc == false ? query.OrderBy(sortProp) : query.OrderByDescending(sortProp);
                }
                else
                {
                    query = bDesc == false ? query.ThenBy(sortProp) : query.ThenByDescending(sortProp);
                }
            }

            return query;
        }

        public static IOrderedQueryable<T> SortBy<T>(this IQueryable<T> query, string sortInfos)
        {
            return ((IOrderedQueryable<T>)query).SortBy(sortInfos);
        }
    }
}
