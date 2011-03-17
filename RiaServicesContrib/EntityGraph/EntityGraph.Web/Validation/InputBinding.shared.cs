using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RiaServicesContrib.Validation
{
    /// <summary>
    /// Class that represent a binding for a validation rule dependency
    /// 
    /// For a validation rule dependency
    ///    A => A.B.c
    ///    
    /// And an object
    ///    var a = new A { B = new B() };
    /// 
    /// TargetOwnerObject equals the object a.B,
    /// ParameterObjectBinding represents the binding of parameter 'A' to object 'a'.
    /// 
    /// </summary>
    internal class ValidationRuleDependencyBinding
    {
        public ValidationRuleDependency ValidationRuleDependency { get; set; }
        private object _targetOwnerObject;
        /// <summary>
        /// Represents the owning object of the target property 'p' of a validation rule dependency 'A => A.some.path.p'.
        /// </summary>
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
        /// <summary>
        /// Represents the binding for the parameter 'A' of a validation rule dependency 'A => A.some.path.p'.
        /// </summary>
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
