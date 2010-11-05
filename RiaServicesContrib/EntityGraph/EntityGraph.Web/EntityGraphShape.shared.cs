using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;

namespace EntityGraph
{
    public static class Test
    {
        /// <summary>
        /// Type mapper from IEnumerable&lt;<typeparamref name="T"/>> to T. Used for specifying a path in an EntityGraphShape from a collection to its elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T to<T>(this IEnumerable<T> list)
        {
            throw new NotImplementedException();
        }
    }

    public class EntityGraphShape<TEntity, TBase> : List<Expression<Func<TEntity, TBase>>> where TEntity : TBase
    {
        public IEnumerable<Type> PathComponents(Expression<Func<TEntity, TBase>> pathExpr)
        {
            return PathComponents(pathExpr.Body);
        }
        private IEnumerable<Type> PathComponents(Expression pathExpr)
        {
            Expression e = pathExpr;

            while(pathExpr != null && pathExpr is ParameterExpression == false)
            {
                Type type = null;
                if(pathExpr is UnaryExpression)
                {
                    var unary = pathExpr as UnaryExpression;
                    if(unary.Operand is MemberExpression)
                    {
                        type = pathExpr.Type;
                        pathExpr = ((MemberExpression)unary.Operand).Expression;
                    }
                }
                else if(pathExpr is MemberExpression)
                {
                    type = pathExpr.Type;
                    pathExpr = ((MemberExpression)pathExpr).Expression;
//                    continue;
                }
                else if(pathExpr is MethodCallExpression)
                {
                    type= pathExpr.Type;
                    pathExpr = ((MethodCallExpression)pathExpr).Arguments.SingleOrDefault();
                    continue;
                }
                else
                {
                    throw new ArgumentException("Invalid expression encountered");
                }
                yield return type;
            }
            yield return pathExpr.Type;
        }
        public IEnumerable<PropertyInfo> OutEdges(TBase entity, string visitedPath)
        {
            return OutEdgesAll(entity.GetType(), visitedPath).Distinct();
        }
        public IEnumerable<PropertyInfo> OutEdges(Type entityType, string visitedPath)
        {
            return OutEdgesAll(entityType, visitedPath).Distinct();
        }
        private IEnumerable<PropertyInfo> OutEdgesAll(Type entityType, string visitedPath)
        {
            string[] visitedPathComponents = visitedPath.Split('.');
            foreach(var pathExpr in this)
            {
                var path = PathContinuation(entityType, visitedPath, pathExpr.Body);
                if(path != null)
                    yield return path;
            }
        }
        private PropertyInfo PathContinuation(Type entityType, string visitedPath, Expression pathExpr)
        {
            var pathToVisit = GetPathComponent(visitedPath, pathExpr);
            if(pathToVisit != null)
            {
                string propertyName = pathToVisit.Split('.').LastOrDefault();
                return entityType.GetProperty(propertyName);
            }
            return null;
        }
        private string GetPathComponent(string visitedPath, Expression pathExpr)
        {
            MemberExpression body = null;
            if(pathExpr is UnaryExpression)
            {
                var unary = pathExpr as UnaryExpression;
                if(unary.Operand is MemberExpression)
                    body = unary.Operand as MemberExpression;
            }
            else if(pathExpr is MemberExpression)
            {
                body = pathExpr as MemberExpression;
            }
            else if(pathExpr is ParameterExpression)
            {
                var objectExpression = pathExpr as ParameterExpression;
                return objectExpression.Name;
            }
            else if(pathExpr is MethodCallExpression)
            {
                var objectExpression = pathExpr as MethodCallExpression;
                var baseExpr = objectExpression.Arguments.SingleOrDefault();
                if(baseExpr != null)
                {
                    return GetPathComponent(visitedPath, baseExpr);
                }
            }

            if(body == null)
                throw new ArgumentException("Invalid expression encountered");
            var path = GetPathComponent(visitedPath, body.Expression);
            if(path == null)
                return null;
            if(path != visitedPath && path.StartsWith(visitedPath))
                return path;
            var npath = path + "." + body.Member.Name;
            if(npath.StartsWith(visitedPath) || (visitedPath.StartsWith(npath) && visitedPath[npath.Length] == '.'))
                return npath;
            return null;
        }
    }
}
