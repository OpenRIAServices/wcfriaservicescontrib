using System;
using System.Linq.Expressions;
using System.Reflection;
namespace EntityGraph.Validation
{
    /// <summary>
    /// Class that represent a binding for an validation rule dependency
    /// </summary>
    internal class ValidationRuleDependencyBinding
    {
        public ValidationRuleDependency ValidationRuleDependency { get; set; }
        private object _targetOwnerObject;
        public object TargetOwnerObject
        {
            get
            {
                if(_targetOwnerObject == null)
                {
                    _targetOwnerObject = GetTargetOwnerObject();
                } 
                return _targetOwnerObject;
            }
        }
        public ParameterObjectBinding ParameterObjectBinding { get; set; }

        private Object GetTargetOwnerObject()
        {
            var obj = ValidationRuleDependency.Expression.Body.Aggregate(
                ParameterObjectBinding.ParameterObject, (s, e) => GetObject(s, e));
            return obj;
        }
        private object GetObject(object seed, Expression expr)
        {
            if(expr is MemberExpression)
            {
                var propInfo = ((MemberExpression)expr).Member as PropertyInfo;
                if(propInfo == ValidationRuleDependency.TargetProperty)
                    return seed;
                return propInfo.GetValue(seed, null);
            }
            return seed;
        }
    }
}
