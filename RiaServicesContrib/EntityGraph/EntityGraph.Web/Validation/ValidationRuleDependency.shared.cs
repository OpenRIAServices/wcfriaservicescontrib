using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("EntityGraphtest")]

namespace EntityGraph.Validation
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
                if(value.Body is MemberExpression == false)
                {
                    throw new Exception("Invalid expression. Expression should be a property path.");
                }
                var tail = ((MemberExpression)value.Body);
                TargetProperty = tail.Member as PropertyInfo;
                TargetPropertyOwnerType = tail.Expression.Type;
                ParameterExpression = tail.Aggregate<ParameterExpression>(null, (x, expr) => x == null ? expr as ParameterExpression : x);
            }
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
