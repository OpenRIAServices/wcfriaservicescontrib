using System;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityGraph.Validation
{
    public static class ExpressionAggregator
    {

        public static TAccumulate Aggregate<TAccumulate>(this Expression expr, TAccumulate seed, Func<TAccumulate, Expression, TAccumulate> func)
        {
            if(expr is ConstantExpression)
            {
                return func(seed, expr);
            }
            if(expr is ParameterExpression)
            {
                return func(seed, expr);
            }
            if(expr is UnaryExpression)
            {
                var uexpr = (UnaryExpression)expr;
                if(uexpr.NodeType == ExpressionType.Convert || uexpr.NodeType == ExpressionType.TypeAs)
                {
                    return Aggregate(uexpr.Operand, seed, func);
                }
                else
                {
                    throw new Exception(expr.ToString() + " is not a valid lambda expression for an EnumerationBinding.");
                }
            }
            if(expr is MemberExpression)
            {
                var memberExpression = expr as MemberExpression;
                if(memberExpression.Member is PropertyInfo)
                {
                    return func(Aggregate(memberExpression.Expression, seed, func), expr);
                }
            }
            throw new Exception("Unsupported expression type: " + expr.GetType().Name);
        }
    }
}
