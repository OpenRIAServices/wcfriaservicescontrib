using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("EntityGraphtest")]

namespace RiaServicesContrib.Validation
{
    /// <summary>
    /// Class that represents a validation rule dependency of a validation rule.
    ///     
    /// Dependencies are expressed in terms of lambda expressions.
    /// </summary>
    public class ValidationRuleDependency
    {
        private LambdaExpression _expression;
        /// <summary>
        /// Gets the validation rule dependency as a Lambda expression.
        /// </summary>
        public LambdaExpression Expression
        {
            get
            {
                return _expression;
            }
            internal set
            {
                _expression = value;
                var tail = GetMemberExpression(value.Body);
                TargetProperty = tail.Member as PropertyInfo;
                TargetPropertyOwnerType = tail.Expression.Type;
                ParameterExpression = tail.Aggregate<ParameterExpression>(null, (x, expr) => x == null ? expr as ParameterExpression : x);
            }
        }
        /// <summary>
        /// Method that returns the MemberExpression of an expression, stripping of
        /// any unary expressions that represent type casts.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private MemberExpression GetMemberExpression(Expression expr)
        {
            if(expr is MemberExpression)
            {
                return expr as MemberExpression;
            }
            if(expr is UnaryExpression)
            {
                var uexpr = (UnaryExpression)expr;
                if(uexpr.NodeType == ExpressionType.Convert || uexpr.NodeType == ExpressionType.TypeAs)
                {
                    return GetMemberExpression(uexpr.Operand);
                }
            }
            throw new Exception(expr.ToString() + " is invalid. Expression should be a property path.");
        }
        /// <summary>
        /// Gets the parameter expression 'A' of a validation rule dependency 'A => A.B.c'.
        /// </summary>
        public ParameterExpression ParameterExpression { get; private set; }
        /// <summary>
        /// Gets the target property 'c' of a validation rule dependency 'A => A.B.c'.
        /// </summary>
        public PropertyInfo TargetProperty { get; private set; }
        /// <summary>
        /// Gets the type of the property 'B' (i.e., the owning proeprty of 'c') of a 
        /// validation rule dependency 'A => A.B.c'.
        /// </summary>
        public Type TargetPropertyOwnerType { get; private set; }
        /// <summary>
        /// Indicates if the the target property 'c' of a validation rule dependency 'A => A.B.c' 
        /// can be invalidated by a validation rule or not.
        /// </summary>
        public bool InputOnly { get; internal set; }
    }
}
