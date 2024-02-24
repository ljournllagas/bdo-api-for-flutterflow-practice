using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Persistence.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> OrderBy<T>(
            this IEnumerable<T> source,
            string property)
        {
            return ApplyOrder(source, property, "OrderBy");
        }

        public static IEnumerable<T> OrderByDescending<T>(
            this IEnumerable<T> source,
            string property)
        {
            return ApplyOrder(source, property, "OrderByDescending");
        }

        public static IEnumerable<T> ThenBy<T>(
            this IOrderedEnumerable<T> source,
            string property)
        {
            return ApplyOrder(source, property, "ThenBy");
        }

        public static IEnumerable<T> ThenByDescending<T>(
            this IOrderedEnumerable<T> source,
            string property)
        {
            return ApplyOrder(source, property, "ThenByDescending");
        }

        static IEnumerable<T> ApplyOrder<T>(
            IEnumerable<T> source,
            string property,
            string methodName)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;


            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Enumerable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda.Compile() });

            return (IEnumerable<T>)result;
        }
    }
}
