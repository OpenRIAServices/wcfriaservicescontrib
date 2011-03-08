using System;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityGraph.Validation
{
    /// <summary>
    /// Class that represents an dependency dependency of a validatino rule
    /// </summary>
    public class ValidationRuleDependency
    {
        private LambdaExpression _expression;
        public LambdaExpression Expression
        {
            get
            {
                return _expression;
            }
            set
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
        public ParameterExpression ParameterExpression { get; private set; }
        public PropertyInfo TargetProperty { get; private set; }
        public Type TargetPropertyOwnerType { get; private set; }
    }
}
