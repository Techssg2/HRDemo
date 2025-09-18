using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Abstracts
{
    public static class LamdaExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string sortExpression)
        {
            if (source == null)
                return source;

            if (string.IsNullOrEmpty(sortExpression))
                return source;

            return System.Linq.Dynamic.Core.DynamicQueryableExtensions.OrderBy(source, sortExpression);
        }
        public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, params object[] args)
        {
            if (source == null)
                return source;

            if (string.IsNullOrEmpty(predicate))
                return source;

            return System.Linq.Dynamic.Core.DynamicQueryableExtensions.Where(source, predicate, args);
        }
        public static IQueryable<T> Include<T>(this IQueryable<T> query, Expression<Func<T, object>> selector) where T : class, new()
        {
            string path = new PropertyPathVisitor().GetPropertyPath(selector);
            return query.Include(path);
        }

        public static Expression<Func<T, bool>> And<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null)
            {
                left = x => true;
            }
            var invokedExpr = Expression.Invoke(right, left.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left.Body, invokedExpr), left.Parameters);
        }

        class PropertyPathVisitor : ExpressionVisitor
        {
            private Stack<string> _stack;

            public string GetPropertyPath(Expression expression)
            {
                _stack = new Stack<string>();
                Visit(expression);
                return _stack
                    .Aggregate(
                        new StringBuilder(),
                        (sb, name) =>
                            (sb.Length > 0 ? sb.Append(".") : sb).Append(name))
                    .ToString();
            }

            protected override Expression VisitMember(MemberExpression expression)
            {
                if (_stack != null)
                    _stack.Push(expression.Member.Name);
                return base.VisitMember(expression);
            }

            protected override Expression VisitMethodCall(MethodCallExpression expression)
            {
                if (IsLinqOperator(expression.Method))
                {
                    for (int i = 1; i < expression.Arguments.Count; i++)
                    {
                        Visit(expression.Arguments[i]);
                    }
                    Visit(expression.Arguments[0]);
                    return expression;
                }
                return base.VisitMethodCall(expression);
            }

            private static bool IsLinqOperator(MethodInfo method)
            {
                if (method.DeclaringType != typeof(Queryable) && method.DeclaringType != typeof(Enumerable))
                    return false;
                return Attribute.GetCustomAttribute(method, typeof(ExtensionAttribute)) != null;
            }
        }
    }
}
